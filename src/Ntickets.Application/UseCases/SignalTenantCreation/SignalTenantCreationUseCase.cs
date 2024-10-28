using Microsoft.FeatureManagement;
using Ntickets.Application.UseCases.Base;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.SignalTenantCreation.Inputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.UseCases.SignalTenantCreation;

public sealed class SignalTenantCreationUseCase : UseCaseFeatureManagedBase, IUseCase<SignalTenantCreationUseCaseInput>
{
    private readonly ITraceManager _traceManager;

    public SignalTenantCreationUseCase(ITraceManager traceManager, IFeatureManager featureManager) : base(featureManager)
    {
        _traceManager = traceManager;
    }

    protected override string FeatureFlagName => nameof(SignalTenantCreationUseCase);

    public Task<MethodResult<INotification>> ExecuteUseCaseAsync(
        SignalTenantCreationUseCaseInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(SignalTenantCreationUseCase)}.{nameof(ExecuteUseCaseAsync)}",
            activityKind: ActivityKind.Internal,
            input: input,
            handler: (input, auditableInfo, activity, cancellationToken) =>
            {

            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
