using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.SignalTenantCreationInfo;
using Ntickets.Application.UseCases.SignalTenantCreationInfo.Inputs;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.Domain.BoundedContexts.EventContext.Events;
using System.Text.Json;

namespace Ntickets.Application.Services.BackgroundServices;

public sealed class CreateTenantEventConsumer : BackgroundService
{
    private readonly IApacheKafkaConsumer _consumer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreateTenantEventConsumer> _logger;

    public CreateTenantEventConsumer(
        IApacheKafkaConsumer consumer, 
        IServiceProvider serviceProvider,
        ILogger<CreateTenantEventConsumer> logger)
    {
        _consumer = consumer;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    private const string TOPIC_NAME = "CREATE_TENANT_EVENT";

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                _consumer.Subscribe(TOPIC_NAME);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var message = _consumer.Consume(stoppingToken);

                    var currentServiceProvider = _serviceProvider.CreateScope();

                    var useCase = currentServiceProvider.ServiceProvider.GetRequiredService<IUseCase<SignalTenantCreationInfoUseCaseInput>>();

                    var @event = JsonSerializer.Deserialize<CreateTenantEvent>(message.Message.Value)!;

                    var useCaseResult = await useCase.ExecuteUseCaseAsync(
                        input: SignalTenantCreationInfoUseCaseInput.Factory(
                            @event: @event),
                        auditableInfo: AuditableInfoValueObject.Factory(
                            correlationId: @event.CorrelationId),
                        cancellationToken: stoppingToken);

                    _logger.LogInformation(
                        message: "[{Type}][{Timestamp}][{TopicName}][{UseCase}] Event = {Event}, IsSuccess = {IsSuccess}, Output = {Output}",
                        nameof(CreateTenantEventConsumer),
                        DateTime.UtcNow,
                        TOPIC_NAME,
                        nameof(SignalTenantCreationInfoUseCase),
                        message.Message.Value,
                        useCaseResult.IsSuccess,
                        JsonSerializer.Serialize(useCaseResult.Notifications));
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    exception: ex,
                    message: "[{Type}][{Timestamp}][{TopicName}] The consumer could not be executed, because an unhandled exception has throwed.",
                    typeof(CreateTenantEventConsumer),
                    DateTime.UtcNow,
                    TOPIC_NAME);
            }
            finally
            {
                _consumer.Close();
            }
        });

        return Task.CompletedTask;
    }
}
