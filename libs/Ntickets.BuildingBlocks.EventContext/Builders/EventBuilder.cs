using Ntickets.BuildingBlocks.EventContext.Interfaces;

namespace Ntickets.BuildingBlocks.EventContext.Builders;

public class EventBuilder<TDescriptor>
{
    public static IEvent<TDescriptor> Factory(string eventId, string eventName, TDescriptor descriptor, string correlationId, DateTime eventTimestamp)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor, correlationId, eventTimestamp);
    public static IEvent<TDescriptor> FactoryUtcNow(string eventId, string eventName, TDescriptor descriptor, string correlationId)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor, correlationId, DateTime.UtcNow);
    public static IEvent<TDescriptor> FactoryNow(string eventId, string eventName, TDescriptor descriptor, string correlationId)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor,  correlationId,DateTime.Now);
}
