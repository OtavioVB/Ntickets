using Ntickets.Application.Services.Internal.TenantContext.Inputs;
using Ntickets.Application.Services.Internal.TenantContext.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;

namespace Ntickets.Application.Services.Internal.TenantContext.Interfaces;

public interface ITenantService
{
    public Task<MethodResult<INotification, CreateTenantServiceOutput>> CreateTenantServiceAsync(
        CreateTenantServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
