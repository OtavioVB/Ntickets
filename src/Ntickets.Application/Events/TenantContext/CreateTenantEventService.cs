using Ntickets.Application.Events.Base;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.EventContext.Builders;
using Ntickets.BuildingBlocks.EventContext.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Utils;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Domain.ValueObjects;
using Ntickets.Infrascructure.RabbitMq.Enumerators;
using Ntickets.Infrascructure.RabbitMq.Interfaces;
using System.Diagnostics;

namespace Ntickets.Application.Events.TenantContext;

public sealed class CreateTenantEventService : EventBaseService<CreateTenantEvent>
{
    public CreateTenantEventService(IRabbitMqPublisher rabbitMqPublisher, ITraceManager traceManager) : base(rabbitMqPublisher, traceManager)
    {
    }

    private const string CREATE_TENANT_EVENT_SERVICE_ROUTING_KEY = "ntickets.tenants.create.*";
    private const string CREATE_TENANT_EVENT_SERVICE_EXCHANGE_NAME = "ntickets.tenants";

    protected override string EventName => "NTICKETS_TENANT_CREATED";

    public override Task PublishEventAsync(CreateTenantEvent @event, AuditableInfoValueObject auditableInfo, CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(CreateTenantEventService)}.{nameof(PublishEventAsync)}",
            activityKind: ActivityKind.Producer,
            input: @event,
            handler: (input, auditableInfo, activity, cancellationToken) 
                => Task.Run(() =>
                {
                    var createTenantEventId = IdValueObject.Factory();

                    var createTenantEvent = EventBuilder<CreateTenantEvent>.FactoryUtcNow(
                        eventId: createTenantEventId,
                        eventName: EventName,
                        descriptor: input,
                        correlationId: auditableInfo.GetCorrelationId());

                    _rabbitMqPublisher.ExchangeDeclare(
                        exchangeName: CREATE_TENANT_EVENT_SERVICE_EXCHANGE_NAME,
                        exchangeType: EnumExchangeType.Topic);

                    _rabbitMqPublisher.PublishMessage(
                        exchangeName: CREATE_TENANT_EVENT_SERVICE_EXCHANGE_NAME,
                        routingKey: CREATE_TENANT_EVENT_SERVICE_ROUTING_KEY,
                        message: createTenantEvent);
                }, cancellationToken),
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: [
                KeyValuePair.Create(
                    key: TraceNames.EVENT_NAME,
                    value: EventName),
                KeyValuePair.Create(
                    key: TraceNames.RABBITMQ_MESSENGER_EXCHANGE_NAME,
                    value: CREATE_TENANT_EVENT_SERVICE_EXCHANGE_NAME),
                KeyValuePair.Create(
                    key: TraceNames.RABBITMQ_MESSENGER_ROUTING_KEY,
                    value: CREATE_TENANT_EVENT_SERVICE_ROUTING_KEY)
                ]);
}
