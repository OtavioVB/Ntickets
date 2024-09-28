using Ntickets.Application.Services.TenantContext.Inputs;
using Ntickets.Application.Services.TenantContext.Interfaces;
using Ntickets.Application.Services.TenantContext.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.Services.TenantContext;

public sealed class TenantService : ITenantService
{
    private readonly ITraceManager _traceManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBaseRepository<Tenant> _tenantBaseRepository;
    private readonly IExtensionTenantRepository _extensionTenantRepository;

    public TenantService(ITraceManager traceManager, IUnitOfWork unitOfWork, IBaseRepository<Tenant> tenantBaseRepository, IExtensionTenantRepository extensionTenantRepository)
    {
        _traceManager = traceManager;
        _unitOfWork = unitOfWork;
        _tenantBaseRepository = tenantBaseRepository;
        _extensionTenantRepository = extensionTenantRepository;
    }

    private const string TENANT_ALREADY_EXISTS_NOTIFICATION_CODE = "TENANT_ALREADY_EXISTS";
    private const string TENANT_ALREADY_EXISTS_NOTIFICATION_MESSAGE = "O whitelabel já possui cadastro na base de dados.";

    public Task<MethodResult<INotification, CreateTenantServiceOutput>> CreateTenantServiceAsync(
        CreateTenantServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(TenantService)}.{nameof(CreateTenantServiceAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var inputValidationResult = input.GetInputValidation();

                if (inputValidationResult.IsError)
                    return MethodResult<INotification, CreateTenantServiceOutput>.FactoryError(
                        notifications: inputValidationResult.Notifications);

                var tenantExistsRepositoryAsync = await _extensionTenantRepository.VerifyTenantExistsByDocumentAsync(
                    document: input.Document.GetDocument(),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                if (tenantExistsRepositoryAsync)
                {
                    var tenantAlreadyExistsNotification = NotificationBuilder.BuildErrorNotification(
                        code: TENANT_ALREADY_EXISTS_NOTIFICATION_CODE,
                        message: TENANT_ALREADY_EXISTS_NOTIFICATION_MESSAGE);

                    return MethodResult<INotification, CreateTenantServiceOutput>.FactoryError(
                        notifications: [tenantAlreadyExistsNotification]);
                }

                var tenant = Tenant.Create(
                    fantasyName: input.FantasyName,
                    legalName: input.LegalName,
                    document: input.Document,
                    email: input.Email,
                    phone: input.Phone);

                await _tenantBaseRepository.AddAsync(
                    entity: tenant,
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                await _unitOfWork.ApplyDataContextTransactionChangeAsync(auditableInfo, cancellationToken);

                const string CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL_NOTIFICATION_CODE = "CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL";
                const string CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL_NOTIFICATION_MESSAGE = "A criação do whitelabel foi executada com sucesso.";

                var tenantCreatedSuccessNotification = NotificationBuilder.BuildSuccessNotification(
                    code: CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL_NOTIFICATION_CODE,
                    message: CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL_NOTIFICATION_MESSAGE);

                return MethodResult<INotification, CreateTenantServiceOutput>.FactorySuccess(
                    notifications: [tenantCreatedSuccessNotification],
                    output: CreateTenantServiceOutput.Factory(
                        tenantId: tenant.TenantId,
                        createdAt: tenant.CreatedAt,
                        status: tenant.Status,
                        fantasyName: tenant.FantasyName,
                        legalName: tenant.LegalName,
                        document: tenant.Document,
                        email: tenant.Email,
                        phone: tenant.Phone,
                        lastModifiedAt: tenant.LastModifiedAt));
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
