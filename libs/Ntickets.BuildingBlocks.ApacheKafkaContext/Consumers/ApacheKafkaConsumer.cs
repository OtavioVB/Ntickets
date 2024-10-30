using Confluent.Kafka;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers.Interfaces;

namespace Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers;

public sealed class ApacheKafkaConsumer : IApacheKafkaConsumer
{
    private readonly IConsumer<string, string> _consumer;

    public ApacheKafkaConsumer(ConsumerConfig consumerConfiguration)
    {
        _consumer = new ConsumerBuilder<string, string>(consumerConfiguration).Build();
    }

    public void Close()
        => _consumer.Close();

    public void Subscribe(string topicName)
        => _consumer.Subscribe(topicName);

    public ConsumeResult<string, string> Consume(CancellationToken cancellationToken = default)
        => _consumer.Consume(cancellationToken);
}
