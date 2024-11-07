using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Ntickets.Application.Services.BackgroundServices;
using Ntickets.Application.Services.External.Discord.Inputs;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.SignalTenantCreationInfo;
using Ntickets.Application.UseCases.SignalTenantCreationInfo.Inputs;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.UnitTests.Domain.BoundedContexts.TenantContext.Events.Faker;
using System.Text.Json;

namespace Ntickets.UnitTests.Application.Services.BackgroundServices;

public sealed class CreateTenantEventConsumerValidationTests
{
    [Fact]
    public async Task Given_Valid_Consume_Message_Sender_Should_Use_Case_Must_Be_Executed_And_Logging_Will_Be_As_Expected()
    {
        // Arrange
        const string EXPECTED_TOPIC_NAME = "CREATE_TENANT_EVENT";

        var loggerMock = new Mock<ILogger<CreateTenantEventConsumer>>();

        var useCaseMock = new Mock<IUseCase<SignalTenantCreationInfoUseCaseInput>>();
        useCaseMock.Setup(p => p.ExecuteUseCaseAsync(
            It.IsAny<SignalTenantCreationInfoUseCaseInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(Task.FromResult(MethodResult<INotification>.FactorySuccess()));

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped((serviceProvider)
            => useCaseMock.Object);

        var consumerMock = new Mock<IApacheKafkaConsumer>();
        var @event = JsonSerializer.Serialize(FakerCreateTenantEvent.CreateInstance());
        consumerMock.Setup(p => p.Consume(It.IsAny<CancellationToken>())).Returns(new ConsumeResult<string, string>()
        {
            Message = new Message<string, string>
            {
                Value = @event
            }
        });

        var consumer = new CreateTenantEventConsumer(
            consumer: consumerMock.Object,
            serviceProvider: serviceCollection.BuildServiceProvider(),
            logger: loggerMock.Object,
            consumerTest: true);

        // Act
        await consumer.StartAsync(
            cancellationToken: CancellationToken.None);
        await Task.Delay(100);
        await consumer.StopAsync(
            cancellationToken: CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(p => p == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t)
                    => v.ToString()!.Contains($"[{nameof(CreateTenantEventConsumer)}]")
                    && v.ToString()!.Contains($"[{EXPECTED_TOPIC_NAME}]")
                    && v.ToString()!.Contains($"[{nameof(SignalTenantCreationInfoUseCase)}]")
                    && v.ToString()!.Contains($"Event = {@event}")
                    && v.ToString()!.Contains($"IsSuccess = {true}")
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Given_Throw_Exception_During_The_Consume_Message_Should_Log_Error()
    {
        // Arrange
        const string EXPECTED_TOPIC_NAME = "CREATE_TENANT_EVENT";
        const string EXPECTED_LOG_ERROR_MESSAGE = "The consumer could not be executed, because an unhandled exception has throwed.";

        var loggerMock = new Mock<ILogger<CreateTenantEventConsumer>>();

        var useCaseMock = new Mock<IUseCase<SignalTenantCreationInfoUseCaseInput>>();
        useCaseMock.Setup(p => p.ExecuteUseCaseAsync(
            It.IsAny<SignalTenantCreationInfoUseCaseInput>(),
            It.IsAny<AuditableInfoValueObject>(),
            It.IsAny<CancellationToken>())).Returns(Task.FromResult(MethodResult<INotification>.FactorySuccess()));

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped((serviceProvider)
            => useCaseMock.Object);

        var consumerMock = new Mock<IApacheKafkaConsumer>();
        var @event = JsonSerializer.Serialize(FakerCreateTenantEvent.CreateInstance());
        consumerMock.Setup(p => p.Consume(It.IsAny<CancellationToken>())).Throws(new Exception());

        var consumer = new CreateTenantEventConsumer(
            consumer: consumerMock.Object,
            serviceProvider: serviceCollection.BuildServiceProvider(),
            logger: loggerMock.Object,
            consumerTest: true);

        // Act
        await consumer.StartAsync(
            cancellationToken: CancellationToken.None);
        await Task.Delay(1000);
        await consumer.StopAsync(
            cancellationToken: CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(p => p == LogLevel.Critical),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t)
                    => v.ToString()!.Contains($"[{nameof(CreateTenantEventConsumer)}]")
                    && v.ToString()!.Contains($"[{EXPECTED_TOPIC_NAME}]")
                    && v.ToString()!.Contains(EXPECTED_LOG_ERROR_MESSAGE)
                ),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
