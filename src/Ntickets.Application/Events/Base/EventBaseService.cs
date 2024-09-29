using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.EventContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Infrascructure.RabbitMq.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.Events.Base;

public abstract class EventBaseService<TEvent> : IEventService<TEvent>
    where TEvent : class
{
    protected readonly IRabbitMqPublisher _rabbitMqPublisher;
    protected readonly ITraceManager _traceManager;

    public EventBaseService(
        IRabbitMqPublisher rabbitMqPublisher,
        ITraceManager traceManager)
    {
        _rabbitMqPublisher = rabbitMqPublisher;
        _traceManager = traceManager;
    }

    protected abstract string EventName { get; }

    public abstract Task PublishEventAsync(
        TEvent @event,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken);
}
