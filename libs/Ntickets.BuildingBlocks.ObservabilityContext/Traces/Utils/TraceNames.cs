namespace Ntickets.BuildingBlocks.ObservabilityContext.Traces.Utils;

public static class TraceNames
{
    public static string UNIT_OF_WORK_TRANSACTION_ISOLATION_LEVEL = "request.database.transaction.isolation.level";
    public static string UNIT_OF_WORK_TRANSACTION_RESULT = "request.database.transaction.commmited.status";
    public static string EVENT_NAME = "request.usecase.event.name";
    public static string RABBITMQ_MESSENGER_EXCHANGE_NAME = "request.messenger.exchange.name";
    public static string RABBITMQ_MESSENGER_ROUTING_KEY = "request.messenger.routing.key";
}
