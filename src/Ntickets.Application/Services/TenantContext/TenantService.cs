using Ntickets.Application.Services.TenantContext.Inputs;
using Ntickets.Application.Services.TenantContext.Interfaces;
using Ntickets.Application.Services.TenantContext.Outputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;

namespace Ntickets.Application.Services.TenantContext;

public sealed class TenantService : ITenantService
{
    public Task<MethodResult<INotification, CreateTenantServiceOutput>> CreateTenantServiceAsync(
        CreateTenantServiceInput input, 
        AuditableInfoValueObject auditableInfo, 
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
