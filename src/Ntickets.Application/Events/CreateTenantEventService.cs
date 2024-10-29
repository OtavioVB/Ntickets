using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Ntickets.Application.Events.Base;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using System.Diagnostics;
using System.Text.Json;

namespace Ntickets.Application.Events;

public sealed class CreateTenantEventService : EventBaseService<CreateTenantEvent>
{
    private readonly ILogger<CreateTenantEventService> _logger;
    private readonly IApacheKafkaProducer _producer;
    private readonly IResiliencePipelineWrapper _resiliencePipeline;

    public CreateTenantEventService(
        IApacheKafkaProducer producer,
        ILogger<CreateTenantEventService> logger,
        IResiliencePipelineWrapper resiliencePipeline,
        ITraceManager traceManager) : base(traceManager)
    {
        _producer = producer;
        _logger = logger;
        _resiliencePipeline = resiliencePipeline;
    }

    public const string EventName = "CREATE_TENANT_EVENT";

    public override Task PublishEventAsync(CreateTenantEvent @event, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(CreateTenantEventService)}.{nameof(PublishEventAsync)}",
            activityKind: ActivityKind.Producer,
            input: @event,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                try
                {
                    await ProduceResilientEventAsync(
                        message: input,
                        cancellationToken: cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        exception: ex,
                        message: "[{Type}][{Timestamp}][{CorrelationId}][Error = 'Is not possible to publish event on apache kafka topic.'][EventName = {EventName}][Event = {Event}]",
                        typeof(CreateTenantEventService),
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss"),
                        auditableInfo.GetCorrelationId(),
                        EventName,
                        JsonSerializer.Serialize(input));
                }
            }, 
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);

    private Task ProduceResilientEventAsync(CreateTenantEvent message, CancellationToken cancellationToken)
        => _resiliencePipeline.GetResiliencePipeline().ExecuteAsync(async (input, cancellationToken) =>
            await _producer.PublishAsync(
                topicName: EventName,
                messageId: message.TenantId,
                message: message,
                cancellationToken: cancellationToken),
            state: message,
            cancellationToken: cancellationToken).AsTask();
}
