using Ntickets.BuildingBlocks.EventContext.Interfaces;

namespace Ntickets.BuildingBlocks.EventContext.Builders;

public class EventBuilder<TDescriptor>
{
    public static IEvent<TDescriptor> Factory(string eventId, string eventName, TDescriptor descriptor, DateTime eventTimestamp)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor, eventTimestamp);
    public static IEvent<TDescriptor> FactoryUtcNow(string eventId, string eventName, TDescriptor descriptor)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor, DateTime.UtcNow);
    public static IEvent<TDescriptor> FactoryNow(string eventId, string eventName, TDescriptor descriptor)
        => Event<TDescriptor>.Factory(eventId, eventName, descriptor, DateTime.Now);
}
