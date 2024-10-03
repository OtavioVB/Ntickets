using Ntickets.Infrascructure.RabbitMq.Enumerators;
using Ntickets.Infrascructure.RabbitMq.Interfaces;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Ntickets.Infrascructure.RabbitMq;

public sealed class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IModel _channel;

    public RabbitMqPublisher(
        string rabbitMqConnectionUserName,
        string rabbitMqConnectionPassword,
        string rabbitMqConnectionVirtualHost,
        string rabbitMqConnectionHostName,
        string rabbitMqConnectionClientProviderName)
    {
        var rabbitMqConnectionFactory = new ConnectionFactory()
        {
            UserName = rabbitMqConnectionUserName,
            Password = rabbitMqConnectionPassword,
            VirtualHost = rabbitMqConnectionVirtualHost,
            HostName = rabbitMqConnectionHostName,
            ClientProvidedName = rabbitMqConnectionUserName,
        };

        using var connection = rabbitMqConnectionFactory.CreateConnection();

        _channel = connection.CreateModel();
    }

    public void QueueDeclare(
        string queueName)
    {
        const bool MESSENGER_RABBIT_MQ_QUEUE_WILL_SURVIVE_ON_RESTART_CONNECTION = true;
        const bool MESSENGER_RABBIT_MQ_QUEUE_DO_NOT_WILL_BE_USED_BY_UNIQUE_CONNECTION = false;
        const bool MESSENGER_RABBIT_MQ_QUEUE_DO_NOT_WILL_BE_DELETED_AFTER_ALL_MESSAGES_ARE_HANDLED = false;

        _channel.QueueDeclare(
            queue: queueName,
            durable: MESSENGER_RABBIT_MQ_QUEUE_WILL_SURVIVE_ON_RESTART_CONNECTION,
            exclusive: MESSENGER_RABBIT_MQ_QUEUE_DO_NOT_WILL_BE_USED_BY_UNIQUE_CONNECTION,
            autoDelete: MESSENGER_RABBIT_MQ_QUEUE_DO_NOT_WILL_BE_DELETED_AFTER_ALL_MESSAGES_ARE_HANDLED);
    }

    public void ExchangeDeclare(
        string exchangeName,
        EnumExchangeType exchangeType)
    {
        const bool MESSENGER_RABBIT_MQ_EXCHANGE_WILL_SURVIVE_ON_RESTART_SERVICE = true;
        const bool MESSENGER_RABBIT_MQ_EXCHANGE_DO_NOT_WILL_BE_DELETED_AFTER_ALL_MESSAGES_ARE_HANDLED = false;

        _channel.ExchangeDeclare(
            exchange: exchangeName,
            type: exchangeType.ToString(),
            durable: MESSENGER_RABBIT_MQ_EXCHANGE_WILL_SURVIVE_ON_RESTART_SERVICE,
            autoDelete: MESSENGER_RABBIT_MQ_EXCHANGE_DO_NOT_WILL_BE_DELETED_AFTER_ALL_MESSAGES_ARE_HANDLED);
    }

    public void QueueBind(
        string queueName,
        string exchangeName,
        string routingKey)
    {
        _channel.QueueBind(
            queue: queueName,
            exchange: exchangeName,
            routingKey: routingKey);
    }

    public void PublishMessage<T>(
        string exchangeName,
        string routingKey,
        T message)
    {
        const bool MESSENGER_RABBIT_MQ_MUST_BE_THROW_EXCEPTION_IF_THE_MESSAGE_WILL_NOT_ROUTED = true;

        _channel.BasicPublish(
            exchange: exchangeName,
            routingKey: routingKey,
            mandatory: MESSENGER_RABBIT_MQ_MUST_BE_THROW_EXCEPTION_IF_THE_MESSAGE_WILL_NOT_ROUTED,
            basicProperties: null,
            body: Encode(message));
    }

    public void PublishMessage<T>(
        string queueName,
        T message)
    {
        const bool MESSENGER_RABBIT_MQ_MUST_BE_THROW_EXCEPTION_IF_THE_MESSAGE_WILL_NOT_ROUTED = true;

        _channel.BasicPublish(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: MESSENGER_RABBIT_MQ_MUST_BE_THROW_EXCEPTION_IF_THE_MESSAGE_WILL_NOT_ROUTED,
            basicProperties: null,
            body: Encode(message));
    }

    private static ReadOnlyMemory<byte> Encode<T>(T message)
        => Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
}
