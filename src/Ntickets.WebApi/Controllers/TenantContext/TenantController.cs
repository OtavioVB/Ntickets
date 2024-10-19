using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntickets.Application.UseCases.Base;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.WebApi.Controllers.Base;
using Ntickets.WebApi.Controllers.TenantContext.Payloads;
using Ntickets.WebApi.Controllers.TenantContext.Sendloads;
using System.Diagnostics;
using System.Net.Mime;
using System.Text.Json;

namespace Ntickets.WebApi.Controllers.TenantContext;

[Route("api/v1/business-intelligence/tenants")]
[ApiController]
public sealed class TenantController : CustomizedControllerBase
{
    private readonly ILogger<TenantController> _logger;

    public TenantController(ILogger<TenantController> logger, ITraceManager traceManager) : base(traceManager)
    {
        _logger = logger;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostCreateTenantAsync(
        [FromServices] IUseCase<CreateTenantUseCaseInput, CreateTenantUseCaseOutput> useCase,
        [FromHeader(Name = CORRELATION_ID_HEADER_KEY)] string correlationId,
        [FromBody] CreateTenantPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId);

        _logger.LogInformation(@"[{Type}][{Timestamp}][CorrelationId = {CorrelationId}][HTTP {Method} {Endpoint}] Request: {Payload}", 
            $"{nameof(TenantController)}.{nameof(HttpPostCreateTenantAsync)}",
            DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
            correlationId,
            HttpContext.Request.Method,
            HttpContext.Request.Path,
            JsonSerializer.Serialize(input));

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TenantController)}.{nameof(HttpPostCreateTenantAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var useCaseResult = await useCase.ExecuteUseCaseAsync(
                    input: CreateTenantUseCaseInput.Factory(
                        fantasyName: input.FantasyName,
                        legalName: input.LegalName,
                        email: input.Email,
                        phone: input.Phone,
                        document: input.Document),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (useCaseResult.IsError)
                    return (IActionResult)StatusCode(
                        statusCode: StatusCodes.Status400BadRequest,
                        value: useCaseResult.Notifications);

                return StatusCode(
                    statusCode: StatusCodes.Status201Created,
                    value: CreateTenantSendloadOutput.Factory(
                        tenantId: useCaseResult.Output.TenantId,
                        createdAt: useCaseResult.Output.CreatedAt,
                        status: useCaseResult.Output.Status,
                        fantasyName: useCaseResult.Output.FantasyName,
                        legalName: useCaseResult.Output.LegalName,
                        document: useCaseResult.Output.Document,
                        email: useCaseResult.Output.Email,
                        phone: useCaseResult.Output.Phone,
                        lastModifiedAt: useCaseResult.Output.LastModifiedAt,
                        notifications: useCaseResult.Notifications));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }
}
