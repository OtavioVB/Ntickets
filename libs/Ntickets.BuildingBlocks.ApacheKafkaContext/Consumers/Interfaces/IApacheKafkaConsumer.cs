using Confluent.Kafka;

namespace Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers.Interfaces;

public interface IApacheKafkaConsumer
{
    public void Subscribe(string topicName);
    public ConsumeResult<string, string> Consume(CancellationToken cancellationToken = default);
    public void Close();
}
