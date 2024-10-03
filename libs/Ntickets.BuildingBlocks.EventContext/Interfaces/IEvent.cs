namespace Ntickets.BuildingBlocks.EventContext.Interfaces;

public interface IEvent<TDescriptor>
{
    public string EventId { get; }
    public string EventName { get; }
    public TDescriptor Descriptor { get; }
    public string CorrelationId { get; }
    public DateTime EventTimestamp { get; }
}
