using Microsoft.EntityFrameworkCore;
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

        #region Entity Framework Core UnitOfWork Configuration

        serviceCollection.AddScoped<IUnitOfWork, DefaultUnitOfWork>();

        #endregion

        #region Repositories Resilience Policies Configuration

        const int MAX_RETRY_ATTEMPTS = 5;
        const int DELAY_BETWEEN_RETRIES_IN_MS = 100;

        const int TIMEOUT_DELAY_IN_MS = 5000;

        const int CIRCUIT_BREAKER_DURATION_IN_MS = 10000;
        const double CIRCUIT_BREAKER_FAILURE_RATIO = 0.25;
        const int CIRCUIT_BREAKER_MINIMUM_THROUGHPUT = 50;

        var resiliencePipeline = FactoryRepositoryResiliencePipeline(
            maxRetryAttempts: MAX_RETRY_ATTEMPTS,
            delayBetweenRetriesInMs: DELAY_BETWEEN_RETRIES_IN_MS,
            timeoutInMs: TIMEOUT_DELAY_IN_MS,
            circuitBreakerDurationInMs: CIRCUIT_BREAKER_DURATION_IN_MS,
            circuitBreakerFailureRatio: CIRCUIT_BREAKER_FAILURE_RATIO,
            circuitBreakerMinimumThroughput: CIRCUIT_BREAKER_MINIMUM_THROUGHPUT);

        #endregion

        #region Entity Framework Core Repositories Configuration

        serviceCollection.AddScoped<IBaseRepository<Tenant>, TenantRepository>((serviceProvider) 
            => new TenantRepository(
                dataContext: serviceProvider.GetRequiredService<DataContext>(),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                resiliencePipeline: resiliencePipeline));
        serviceCollection.AddScoped<IExtensionTenantRepository, TenantRepository>((serviceProvider)
            => new TenantRepository(
                dataContext: serviceProvider.GetRequiredService<DataContext>(),
                traceManager: serviceProvider.GetRequiredService<ITraceManager>(),
                resiliencePipeline: resiliencePipeline));

        #endregion

        #region RabbitMq Connection Configuration

        #endregion
    }

    private static ResiliencePipeline FactoryRepositoryResiliencePipeline(
        int maxRetryAttempts,
        int delayBetweenRetriesInMs,
        int timeoutInMs,
        int circuitBreakerDurationInMs,
        double circuitBreakerFailureRatio,
        int circuitBreakerMinimumThroughput)
    {
        var resiliencePipelineBuilder = new ResiliencePipelineBuilder();

        var retryOptions = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            MaxRetryAttempts = maxRetryAttempts,
            BackoffType = DelayBackoffType.Linear,
            Delay = TimeSpan.FromMilliseconds(delayBetweenRetriesInMs)
        };

        var timeoutOptions = new TimeoutStrategyOptions()
        {
            Timeout = TimeSpan.FromMilliseconds(timeoutInMs)
        };

        var circuitBreakerOptions = new CircuitBreakerStrategyOptions()
        {
            BreakDuration = TimeSpan.FromSeconds(circuitBreakerDurationInMs),
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            FailureRatio = circuitBreakerFailureRatio,
            MinimumThroughput = circuitBreakerMinimumThroughput
        };

        resiliencePipelineBuilder
            .AddTimeout(timeoutOptions)
            .AddRetry(retryOptions)
            .AddCircuitBreaker(circuitBreakerOptions);

        return resiliencePipelineBuilder.Build();
    }
}
