using Microsoft.FeatureManagement;
using Ntickets.Application.Services.External.Discord.Inputs;
using Ntickets.Application.Services.External.Discord.Interfaces;
using Ntickets.Application.UseCases.Base;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.SignalTenantCreationInfo.Inputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Builders;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.UseCases.SignalTenantCreationInfo;

public sealed class SignalTenantCreationInfoUseCase : UseCaseFeatureManagedBase, IUseCase<SignalTenantCreationInfoUseCaseInput>
{
    private readonly ITraceManager _traceManager;
    private readonly IDiscordService _discordService;

    public SignalTenantCreationInfoUseCase(
        ITraceManager traceManager,
        IDiscordService discordService,
        IFeatureManager featureManager) : base(featureManager)
    {
        _traceManager = traceManager;
        _discordService = discordService;
    }

    protected override string FeatureFlagName => nameof(SignalTenantCreationInfoUseCase);

    public Task<MethodResult<INotification>> ExecuteUseCaseAsync(
        SignalTenantCreationInfoUseCaseInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(SignalTenantCreationInfoUseCase)}.{nameof(ExecuteUseCaseAsync)}",
            activityKind: ActivityKind.Consumer,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                if (!await CanHandleFeatureAsync())
                {
                    const string SIGNAL_TENANT_CREATION_INFO_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_CODE = "SIGNAL_TENANT_CREATION_INFO_FEATURE_FLAG_IS_NOT_ENABLED";
                    const string SIGNAL_TENANT_CREATION_INFO_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_MESSAGE = "O processamento da notificação do evento de criação do contratante não está habilitado para execução.";

                    return MethodResult<INotification>.FactoryError(
                        notifications: [
                            NotificationBuilder.BuildErrorNotification (
                                code: SIGNAL_TENANT_CREATION_INFO_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_CODE,
                                message: SIGNAL_TENANT_CREATION_INFO_FEATURE_FLAG_IS_NOT_ENABLED_NOTIFICATION_MESSAGE)]);
                }

                const string SIGNAL_TENANT_CREATION_INFO_SUCCESS_NOTIFICATION_CODE = "SIGNAL_TENANT_CREATION_INFO_SUCCESS";
                const string SIGNAL_TENANT_CREATION_INFO_SUCCESS_NOTIFICATION_MESSAGE = "O processamento da notificação do evento de criação do contratante foram realizadas com sucesso.";

                _ = _discordService.SignalCreateTenantEventInfoOnChannelAsync(
                    input: SignalCreateTenantEventInfoOnChannelDiscordServiceInput.Factory(
                        tenantId: input.Event.TenantId,
                        fantasyName: input.Event.FantasyName,
                        legalName: input.Event.LegalName,
                        phone: input.Event.Phone,
                        email: input.Event.Email),
                    auditableInfo: auditableInfo,
                    cancellationToken: cancellationToken);

                return MethodResult<INotification>.FactorySuccess(
                    notifications: [
                        NotificationBuilder.BuildSuccessNotification(
                            code: SIGNAL_TENANT_CREATION_INFO_SUCCESS_NOTIFICATION_CODE,
                            message: SIGNAL_TENANT_CREATION_INFO_SUCCESS_NOTIFICATION_MESSAGE)]);
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken);
}
