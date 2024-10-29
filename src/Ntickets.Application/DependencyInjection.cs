using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ntickets.Application.Events;
using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.Application.Services.External.Discord;
using Ntickets.Application.Services.External.Discord.Interfaces;
using Ntickets.Application.Services.External.Discord.Options;
using Ntickets.Application.Services.Internal.TenantContext;
using Ntickets.Application.Services.Internal.TenantContext.Interfaces;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.CreateTenant;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ResilienceContext;
using Ntickets.BuildingBlocks.ResilienceContext.Options;
using Ntickets.BuildingBlocks.ResilienceContext.Options.HttpClientRequests;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.Application;

public static class DependencyInjection
{
    private const string APACHE_KAFKA_RESILIENCE_PIPELINE_WRAPPER_DEFINITION_NAME = "APACHE_KAFKA_RESILIENCE_PIPELINE_WRAPPER";

    public static void ApplyApplicationDependenciesConfiguration(this IServiceCollection serviceCollection,
        DiscordServiceOptions discordServiceOptions)
    {
        #region External Services Dependencies Configuration

        serviceCollection.AddSingleton<IDiscordService, DiscordService>((serviceProvider)
            => new DiscordService(
                metricManager: serviceProvider.GetRequiredService<IMetricManager>(),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                options: discordServiceOptions));

        #endregion

        #region Services Dependencies Configuration

        serviceCollection.AddScoped<ITenantService, TenantService>();

        #endregion

        #region Event Services Depedencies Configuration

        serviceCollection.AddSingleton<IEventService<CreateTenantEvent>, CreateTenantEventService>((serviceProvider)
            => new CreateTenantEventService(
                kafkaProducer: serviceProvider.GetRequiredService<IProducer<Null, string>>(),
                logger: serviceProvider.GetRequiredService<ILogger<CreateTenantEventService>>(),
                resiliencePipeline: serviceProvider.GetRequiredKeyedService<IResiliencePipelineWrapper>(APACHE_KAFKA_RESILIENCE_PIPELINE_WRAPPER_DEFINITION_NAME),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>()));

        #endregion

        #region Use Cases Dependencies Configuration

        serviceCollection.AddScoped<IUseCase<CreateTenantUseCaseInput, CreateTenantUseCaseOutput>, CreateTenantUseCase>();

        #endregion

    }
}
