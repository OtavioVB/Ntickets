﻿using Microsoft.FeatureManagement;
using Ntickets.Application.Events;
using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.Application.Services.Internal.TenantContext.Inputs;
using Ntickets.Application.Services.Internal.TenantContext.Interfaces;
using Ntickets.Application.UseCases.Base;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.EventContext.Builders;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using System.Data;
using System.Diagnostics;

namespace Ntickets.Application.UseCases.CreateTenant;

public sealed class CreateTenantUseCase : UseCaseFeatureManagedBase, IUseCase<CreateTenantUseCaseInput, CreateTenantUseCaseOutput>
{
    private readonly ITraceManager _traceManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITenantService _tenantService;
    private readonly IMetricManager _metricManager;
    private readonly IEventService<CreateTenantEvent> _eventService;

    public CreateTenantUseCase(
        ITraceManager traceManager, 
        IUnitOfWork unitOfWork, 
        ITenantService tenantService, 
        IMetricManager metricManager,
        IFeatureManager featureManager,
        IEventService<CreateTenantEvent> eventService)
        : base(featureManager)
    {
        _traceManager = traceManager;
        _unitOfWork = unitOfWork;
        _tenantService = tenantService;
        _metricManager = metricManager;
        _eventService = eventService;
    }

    private const string USE_CASE_COUNTER_NAME = "ntickets_create_tenant_usecase_count";
    private const string USE_CASE_COUNTER_INDICATIVE_SUCCESS_TAG = "success";

    protected override string FeatureFlagName => nameof(CreateTenantUseCase);

    public Task<MethodResult<INotification, CreateTenantUseCaseOutput>> ExecuteUseCaseAsync(
        CreateTenantUseCaseInput input, 
        AuditableInfoValueObject auditableInfo, 
        CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(CreateTenantUseCase)}.{nameof(ExecuteUseCaseAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: (input, auditableInfo, activity, cancellationToken) 
                => _unitOfWork.ExecuteUnitOfWorkAsync(
                    input: input,
                    handler: async (input, auditableInfo, cancellationToken) =>
                    {
                        if (!await CanHandleFeatureAsync())
                        {
                            const string CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_CODE = "CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED";
                            const string CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_MESSAGE = "A funcionalidade de criação do contratante não está habilitada.";

                            return (false, MethodResult<INotification, CreateTenantUseCaseOutput>.FactoryError(
                                notifications: [
                                    NotificationBuilder.BuildErrorNotification(
                                        code: CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_CODE,
                                        message: CREATE_TENANT_USE_CASE_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_MESSAGE)
                                    ]));
                        }

                        var createTenantServiceResult = await _tenantService.CreateTenantServiceAsync(
                            input: CreateTenantServiceInput.Factory(
                                fantasyName: input.FantasyName,
                                legalName: input.LegalName,
                                email: input.Email,
                                phone: input.Phone,
                                document: input.Document),
                            auditableInfo: auditableInfo,
                            cancellationToken: cancellationToken);

                        if (createTenantServiceResult.IsError)
                            return (false, MethodResult<INotification, CreateTenantUseCaseOutput>.FactoryError(
                                notifications: createTenantServiceResult.Notifications));

                        _metricManager.CreateIfNotExistsAndIncrementCounter(
                            counterName: USE_CASE_COUNTER_NAME,
                            keyValuePairs: new KeyValuePair<string, object?>(
                                key: USE_CASE_COUNTER_INDICATIVE_SUCCESS_TAG,
                                value: true));

                        await _eventService.PublishEventAsync(
                            @event: new CreateTenantEvent(
                                tenantId: createTenantServiceResult.Output.TenantId,
                                createdAt: createTenantServiceResult.Output.CreatedAt,
                                fantasyName: createTenantServiceResult.Output.FantasyName,
                                legalName: createTenantServiceResult.Output.LegalName,
                                document: createTenantServiceResult.Output.Document,
                                email: createTenantServiceResult.Output.Email,
                                phone: createTenantServiceResult.Output.Phone,
                                status: createTenantServiceResult.Output.Status,
                                lastModifiedAt: createTenantServiceResult.Output.LastModifiedAt,
                                eventName: CreateTenantEventService.EventName,
                                correlationId: auditableInfo.GetCorrelationId()),
                            auditableInfo: auditableInfo,
                            cancellationToken: cancellationToken);

                        return (true, MethodResult<INotification, CreateTenantUseCaseOutput>.FactorySuccess(
                            notifications: createTenantServiceResult.Notifications,
                            output: CreateTenantUseCaseOutput.Factory(
                                tenantId: createTenantServiceResult.Output.TenantId,
                                createdAt: createTenantServiceResult.Output.CreatedAt,
                                status: createTenantServiceResult.Output.Status,
                                fantasyName: createTenantServiceResult.Output.FantasyName,
                                legalName: createTenantServiceResult.Output.LegalName,
                                document: createTenantServiceResult.Output.Document,
                                email: createTenantServiceResult.Output.Email,
                                phone: createTenantServiceResult.Output.Phone,
                                lastModifiedAt: createTenantServiceResult.Output.LastModifiedAt)));
                    },
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken,
                    isolationLevel: IsolationLevel.RepeatableRead),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
