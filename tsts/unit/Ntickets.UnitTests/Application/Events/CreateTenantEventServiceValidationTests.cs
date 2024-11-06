using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Ntickets.Application.Events;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.UnitTests.Common;
using Ntickets.UnitTests.Domain.BoundedContexts.TenantContext.Events.Faker;

namespace Ntickets.UnitTests.Application.Events;

public sealed class CreateTenantEventServiceValidationTests
{
    [Fact]
    public async Task Given_Request_To_Create_Tenant_Event_Should_Publish_Event_Message_On_ApacheKafka()
    {
        // Arrange
        const string EXPECTED_TOPIC_NAME = "CREATE_TENANT_EVENT";

        var producerMock = new Mock<IApacheKafkaProducer>();
        var loggerMock = new Mock<ILogger<CreateTenantEventService>>();

        var resiliencePipelineWrapper = ResiliencePipelineWrapper.Build(
            resiliencePipelineDefinitionName: string.Empty,
            options: FakerResiliencePipelineWrapperOptions.CreateInstance());

        var eventService = new CreateTenantEventService(
            producer: producerMock.Object,
            logger: loggerMock.Object,
            resiliencePipeline: resiliencePipelineWrapper,
            traceManager: FakerTraceManager.CreateInstance());

        var @event = FakerCreateTenantEvent.CreateInstance();

        // Act
        await eventService.PublishEventAsync(
            @event: @event,
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        producerMock.Verify(
            p => p.PublishAsync(
                It.Is<string>(p => p == EXPECTED_TOPIC_NAME),
                It.IsAny<string>(),
                It.Is<CreateTenantEvent>(p => @event.Equals(p)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Given_Throws_Exception_On_Request_To_Create_Tenant_Event_Should_Not_Publish_Event_Message_On_ApacheKafka_And_Register_Log()
    {
        // Arrange
        const string EXPECTED_TOPIC_NAME = "CREATE_TENANT_EVENT";
        const string EXPECTED_LOGGING_MESSAGE = "Is not possible to publish event on apache kafka topic.";

        var producerMock = new Mock<IApacheKafkaProducer>();
        producerMock.Setup(p => p.PublishAsync(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CreateTenantEvent>(),
            It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

        var loggerMock = new Mock<ILogger<CreateTenantEventService>>();

        var resiliencePipelineWrapper = ResiliencePipelineWrapper.Build(
            resiliencePipelineDefinitionName: string.Empty,
            options: FakerResiliencePipelineWrapperOptions.CreateInstance());

        var eventService = new CreateTenantEventService(
            producer: producerMock.Object,
            logger: loggerMock.Object,
            resiliencePipeline: resiliencePipelineWrapper,
            traceManager: FakerTraceManager.CreateInstance());

        var @event = FakerCreateTenantEvent.CreateInstance();

        // Act
        await eventService.PublishEventAsync(
            @event: @event,
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(level => level == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(EXPECTED_TOPIC_NAME) && v.ToString()!.Contains(EXPECTED_LOGGING_MESSAGE)),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
