using Ntickets.Application.Services.Internal.TenantContext.Inputs;
using Ntickets.Application.Services.Internal.TenantContext.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.Application.Services.Internal.TenantContext.Interfaces;

public interface ITenantService
{
    public Task<MethodResult<INotification, CreateTenantServiceOutput>> CreateTenantServiceAsync(
        CreateTenantServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
