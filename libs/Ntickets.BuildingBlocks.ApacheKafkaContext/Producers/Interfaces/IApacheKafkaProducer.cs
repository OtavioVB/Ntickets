namespace Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;

public interface IApacheKafkaProducer
{
    public Task PublishAsync<TMessage>(
        string topicName,
        string messageId,
        TMessage message,
        CancellationToken cancellationToken = default);
}
