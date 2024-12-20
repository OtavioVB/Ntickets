﻿using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ntickets.Application.Events;
using Ntickets.Application.Events.Base.Interfaces;
using Ntickets.Application.Services.BackgroundServices;
using Ntickets.Application.Services.External.Discord;
using Ntickets.Application.Services.External.Discord.Interfaces;
using Ntickets.Application.Services.External.Discord.Options;
using Ntickets.Application.Services.Internal.TenantContext;
using Ntickets.Application.Services.Internal.TenantContext.Interfaces;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.CreateTenant;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.Application.UseCases.SignalTenantCreationInfo;
using Ntickets.Application.UseCases.SignalTenantCreationInfo.Inputs;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Consumers.Interfaces;
using Ntickets.BuildingBlocks.ApacheKafkaContext.Producers.Interfaces;
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
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                options: discordServiceOptions,
                logger: serviceProvider.GetRequiredService<ILogger<DiscordService>>()));

        #endregion

        #region Services Dependencies Configuration

        serviceCollection.AddScoped<ITenantService, TenantService>();

        #endregion

        #region Event Services Depedencies Configuration

        serviceCollection.AddSingleton<IEventService<CreateTenantEvent>, CreateTenantEventService>((serviceProvider)
            => new CreateTenantEventService(
                producer: serviceProvider.GetRequiredService<IApacheKafkaProducer>(),
                logger: serviceProvider.GetRequiredService<ILogger<CreateTenantEventService>>(),
                resiliencePipeline: serviceProvider.GetRequiredKeyedService<IResiliencePipelineWrapper>(APACHE_KAFKA_RESILIENCE_PIPELINE_WRAPPER_DEFINITION_NAME),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>()));

        #endregion

        #region Use Cases Dependencies Configuration

        serviceCollection.AddScoped<IUseCase<CreateTenantUseCaseInput, CreateTenantUseCaseOutput>, CreateTenantUseCase>();
        serviceCollection.AddScoped<IUseCase<SignalTenantCreationInfoUseCaseInput>, SignalTenantCreationInfoUseCase>();

        #endregion

        #region Background Services

        const string CREATE_TENANT_EVENT_GROUP_CONSUMER_ORIGIN = nameof(CREATE_TENANT_EVENT_GROUP_CONSUMER_ORIGIN);
        serviceCollection.AddHostedService<CreateTenantEventConsumer>((serviceProvider)
            => new CreateTenantEventConsumer(
                consumer: serviceProvider.GetRequiredKeyedService<IApacheKafkaConsumer>(CREATE_TENANT_EVENT_GROUP_CONSUMER_ORIGIN),
                serviceProvider: serviceProvider,
                logger: serviceProvider.GetRequiredService<ILogger<CreateTenantEventConsumer>>()));

        #endregion

    }
}
