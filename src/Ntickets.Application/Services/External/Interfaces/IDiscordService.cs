using Ntickets.Application.Services.External.Inputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;

namespace Ntickets.Application.Services.External.Interfaces;

public interface IDiscordService
{
    public Task SignalCreateTenantEventInfoOnChannelAsync(
        SignalCreateTenantEventInfoOnChannelDiscordServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
