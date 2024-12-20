﻿using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.EventContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.Events.Base;

public abstract class EventBaseService<TEvent> : IEventService<TEvent>
    where TEvent : class
{
    protected readonly ITraceManager _traceManager;

    protected EventBaseService(
        ITraceManager traceManager)
    {
        _traceManager = traceManager;
    }

    public abstract Task PublishEventAsync(
        TEvent @event,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
