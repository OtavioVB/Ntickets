﻿using Ntickets.Application.Services.External.Discord.Inputs;
using Ntickets.BuildingBlocks.AuditableInfoContext;

namespace Ntickets.Application.Services.External.Discord.Interfaces;

public interface IDiscordService
{
    public Task SignalCreateTenantEventInfoOnChannelAsync(
        SignalCreateTenantEventInfoOnChannelDiscordServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
