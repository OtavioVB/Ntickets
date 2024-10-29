using Confluent.Kafka;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;
using System.Text.Json;

namespace Ntickets.BuildingBlocks.ApacheKafkaContext.Producers;

public sealed class ApacheKafkaProducer : IApacheKafkaProducer
{
    private readonly IProducer<string, string> _producer;

    public ApacheKafkaProducer(ProducerConfig configuration)
    {
        _producer = new ProducerBuilder<string, string>(configuration).Build();
    }

    public Task PublishAsync<TMessage>(
        string topicName,
        string messageId,
        TMessage message,
        CancellationToken cancellationToken = default)
        => _producer.ProduceAsync(
            topic: topicName,
            message: new Message<string, string>()
            {
                Key = messageId,
                Value = JsonSerializer.Serialize(message)
            },
            cancellationToken: cancellationToken);
}
