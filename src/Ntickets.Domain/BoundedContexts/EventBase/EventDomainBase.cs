namespace Ntickets.Domain.BoundedContexts.EventBase;

public abstract record EventDomainBase
{
    public string EventName { get; set; }
    public string CorrelationId { get; set; }

    protected EventDomainBase(string eventName, string correlationId)
    {
        EventName = eventName;
        CorrelationId = correlationId;
    }
}
