using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.EventContext.Interfaces;

namespace Ntickets.Application.Events.Base.Interfaces;

public interface IEventService<TEvent>
    where TEvent : class
{
    public abstract Task PublishEventAsync(
       TEvent @event,
       AuditableInfoValueObject auditableInfo,
       CancellationToken cancellationToken);
}
