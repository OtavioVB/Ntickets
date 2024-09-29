using Ntickets.BuildingBlocks.EventContext.Interfaces;
using System.Text;
using System.Text.Json;

namespace Ntickets.BuildingBlocks.EventContext;

public class Event<TDescriptor> : IEvent<TDescriptor>
{
    public string EventId { get; }
    public string EventName { get; }
    public TDescriptor Descriptor { get; }
    public string CorrelationId { get; }
    public DateTime EventTimestamp { get; }

    private Event(string eventId, string eventName, TDescriptor descriptor, string correlationId, DateTime eventTimestamp)
    {
        EventId = eventId;
        EventName = eventName;
        Descriptor = descriptor;
        CorrelationId = correlationId;
        EventTimestamp = eventTimestamp;
    }

    private const string EVENT_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE = "O ID do evento não pode ser vazio ou conter apenas espaços em branco.";
    private const string EVENT_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE = "O nome do evento não pode ser vazio ou conter apenas espaços em branco.";
    private const string EVENT_TIMESTAMP_COULD_NOT_BE_OUT_OF_RANGE_EXCEPTION_MESSAGE = "O carimbo de data e hora do evento não pode ser inválido ou fora do intervalo esperado. ";

    public static IEvent<TDescriptor> Factory(string eventId, string eventName, TDescriptor descriptor, string correlationId, DateTime eventTimestamp)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            throw new ArgumentNullException(nameof(eventId), EVENT_ID_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE);

        if (string.IsNullOrWhiteSpace(eventName))
            throw new ArgumentNullException(nameof(eventName), EVENT_NAME_COULD_NOT_BE_EMPTY_OR_WHITESPACE_EXCEPTION_MESSAGE);

        if (DateTime.MinValue >= eventTimestamp || DateTime.MaxValue <= eventTimestamp || DateTime.UnixEpoch == eventTimestamp)
            throw new ArgumentOutOfRangeException(nameof(EventTimestamp), EVENT_TIMESTAMP_COULD_NOT_BE_OUT_OF_RANGE_EXCEPTION_MESSAGE);

        return new Event<TDescriptor>(
            eventId: eventId,
            eventName: eventName,
            descriptor: descriptor,
            correlationId: correlationId,
            eventTimestamp: eventTimestamp);
    }

    public string GetEventSerialized()
        => JsonSerializer.Serialize(this);
}
