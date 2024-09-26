using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.WebApi.Controllers.Base;
using Ntickets.WebApi.Controllers.TenantContext.Payloads;
using System.Diagnostics;
using System.Net.Mime;

namespace Ntickets.WebApi.Controllers.TenantContext;

[Route("api/v1/business-intelligence/tenants")]
[ApiController]
public sealed class TenantController : CustomizedControllerBase
{
    public TenantController(ITraceManager traceManager) : base(traceManager)
    {
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    public Task<IActionResult> HttpPostCreateTenantAsync(
        [FromHeader(Name = CORRELATION_ID_HEADER_KEY)] string correlationId,
        [FromBody] CreateTenantPayloadInput input,
        CancellationToken cancellationToken)
    {
        var auditableInfo = AuditableInfoValueObject.Factory(
            correlationId: correlationId);

        return _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TenantController)}.{nameof(HttpPostCreateTenantAsync)}",
            activityKind: ActivityKind.Server,
            input: input,
            handler: (input, auditableInfo, activity, cancellationToken) =>
            {
                return Task.FromResult((IActionResult)Ok());
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
    }
}
