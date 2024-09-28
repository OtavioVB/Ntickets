using Ntickets.Infrascructure.RabbitMq.Enumerators;

namespace Ntickets.Infrascructure.RabbitMq.Interfaces;

public interface IRabbitMqPublisher
{
    public void QueueDeclare(string queueName);
    public void ExchangeDeclare(
        string exchangeName,
        EnumExchangeType exchangeType);
    public void QueueBind(
        string queueName,
        string exchangeName,
        string routingKey);

    public void PublishMessage<T>(
        string exchangeName,
        string routingKey,
        T message);

    public void PublishMessage<T>(
        string queueName,
        T message);
}
