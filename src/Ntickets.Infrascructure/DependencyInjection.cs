﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using Ntickets.BuildingBlocks.ResilienceContext;
using Ntickets.BuildingBlocks.ResilienceContext.Options;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using System.Net.Sockets;
using System.Collections.Immutable;
using Npgsql;

namespace Ntickets.Infrascructure;

public static class DependencyInjection
{
    public static void ApplyInfrascructureDependenciesConfiguration(
        this IServiceCollection serviceCollection,
        string connectionString,
        string rabbitMqConnectionUserName, 
        string rabbitMqConnectionPassword,
        string rabbitMqConnectionVirtualHost,
        string rabbitMqConnectionHostName,
        string rabbitMqConnectionClientProviderName,
        bool configureDbContextInMemory = false)
    {
        #region Entity Framework Core DbContext Configuration

        const int DEFAULT_DB_CONTEXT_POOL_SIZE = 512;

        if (!configureDbContextInMemory)
        {
            const string DB_CONTEXT_ASSEMBLY_MIGRATIONS = "Ntickets.Infrascructure";

            serviceCollection.AddDbContextPool<DataContext>(
                optionsAction: options => options.UseNpgsql(
                    connectionString: connectionString,
                    npgsqlOptionsAction: npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                        assemblyName: DB_CONTEXT_ASSEMBLY_MIGRATIONS)),
                poolSize: DEFAULT_DB_CONTEXT_POOL_SIZE);
        }
        else
        {
            const string IN_MEMORY_DATABASE_NAME = "in_memory_database";

            serviceCollection.AddDbContextPool<DataContext>(
                optionsAction: options => 
                {
                    options.UseInMemoryDatabase(
                        databaseName: IN_MEMORY_DATABASE_NAME);
                    options.EnableSensitiveDataLogging(
                        sensitiveDataLoggingEnabled: true);
                    options.ConfigureWarnings(warningOptions => {
                        warningOptions.Ignore(InMemoryEventId.TransactionIgnoredWarning);
                        warningOptions.Log(InMemoryEventId.TransactionIgnoredWarning);
                    });
                },
                poolSize: DEFAULT_DB_CONTEXT_POOL_SIZE);
        }

        #endregion

        #region Entity Framework Core Repositories Configuration

        const string ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME = nameof(ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME);

        serviceCollection.AddKeyedResiliencePipelineWrapper(
            definitionName: ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME,
            optionsAction: (options) =>
            {
                var exceptionsMustBeHandledCollection = new Type[]
                    {
                        typeof(SocketException),
                        typeof(PostgresException),
                        typeof(NpgsqlException),
                        typeof(TimeoutException),
                        typeof(InvalidOperationException)
                    }.ToImmutableArray();

                options.TimeoutOptions = new ResiliencePipelineTimeoutWrapperOptions()
                {
                    Timeout = TimeSpan.FromSeconds(5)
                };

                options.RetryOptions = new ResiliencePipelineRetryWrapperOptions()
                {
                    MaxRetryAttempts = 5,
                    DelayBetweenRetriesInMiliseconds = 250,
                    HandleExceptionsCollection = exceptionsMustBeHandledCollection
                };

                options.CircuitBreakerOptions = new ResiliencePipelineCircuitBreakerWrapperOptions()
                {
                    BreakDurationInSeconds = 5,
                    MinimumThroughput = 50,
                    FailureRatio = 0.10,
                    HandleExceptionsCollection = exceptionsMustBeHandledCollection
                };
            });

        serviceCollection.AddScoped<IBaseRepository<Tenant>, TenantRepository>((serviceProvider)
            => new TenantRepository(
                dataContext: serviceProvider.GetRequiredService<DataContext>(),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                resiliencePipeline: serviceProvider.GetRequiredKeyedService<IResiliencePipelineWrapper>(ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME)));
        serviceCollection.AddScoped<IExtensionTenantRepository, TenantRepository>((serviceProvider)
            => new TenantRepository(
                dataContext: serviceProvider.GetRequiredService<DataContext>(),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                resiliencePipeline: serviceProvider.GetRequiredKeyedService<IResiliencePipelineWrapper>(ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME)));

        #endregion

        #region Entity Framework Core UnitOfWork Configuration

        serviceCollection.AddScoped<IUnitOfWork, DefaultUnitOfWork>((serviceProvider)
            => new DefaultUnitOfWork(
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                dataContext: serviceProvider.GetRequiredService<DataContext>(),
                resiliencePipelineWrapper: serviceProvider.GetRequiredKeyedService<IResiliencePipelineWrapper>(ENTITY_FRAMEWORK_CORE_REPOSITORIES_RESILIENCE_PIPELINE_NAME)));

        #endregion

        #region RabbitMq Connection Configuration

        #endregion
    }
}
