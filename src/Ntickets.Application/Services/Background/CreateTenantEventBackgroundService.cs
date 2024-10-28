using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ntickets.Application.Events;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using System.Text.Json;

namespace Ntickets.Application.Services.Background;

public sealed class CreateTenantEventBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<Null, string> _consumer;
    private readonly ILogger<CreateTenantEventBackgroundService> _logger;

    public CreateTenantEventBackgroundService(IServiceProvider serviceProvider, IConsumer<Null, string> consumer, ILogger<CreateTenantEventBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _consumer = consumer;
        _logger = logger;
    }

    public const string GroupId = "CREATE_TENANT_EVENT_CONSUMER_ORIGIN";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Subscribe(CreateTenantEventService.EventName);

        _ = Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumedMessage = _consumer.Consume(stoppingToken);

                var @event = JsonSerializer.Deserialize<CreateTenantEvent>(consumedMessage.Message.Value);

                _logger.LogInformation("[ApacheKafka - Consumer][{Type}][{Timestamp}][TopicName = {TopicName}][GroupId = {GroupId}][Event = {Event}]",
                    typeof(CreateTenantEventBackgroundService),
                    DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    CreateTenantEventService.EventName,
                    GroupId,
                    consumedMessage.Message.Value);
            }
        }, stoppingToken);
        
        return Task.CompletedTask;
    }
}
