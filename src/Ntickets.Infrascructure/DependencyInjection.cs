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

        #region Entity Framework Core Repositories Configuration

        var resiliencePipeline = FactoryRepositoryResiliencePipeline();

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

    private static ResiliencePipeline FactoryRepositoryResiliencePipeline()
    {
        var resiliencePipelineBuilder = new ResiliencePipelineBuilder();

        var retryOptions = new RetryStrategyOptions()
        {
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            MaxRetryAttempts = 3,
            BackoffType = DelayBackoffType.Linear,
            Delay = TimeSpan.FromMilliseconds(100),
            OnRetry = (teste) =>
            {
                Console.WriteLine($"\n\n[TENTATIVA {teste.AttemptNumber}][{DateTime.UtcNow.ToUniversalTime().ToString()}]\n\n");

                return ValueTask.CompletedTask;
            }
        };

        var timeoutOptions = new TimeoutStrategyOptions()
        {
            Timeout = TimeSpan.FromMilliseconds(5000)
        };

        var circuitBreakerOptions = new CircuitBreakerStrategyOptions()
        {
            OnOpened = (teste) =>
            {
                Console.WriteLine($"\n\n[CIRCUIT BREAKER ABERTO][{DateTime.UtcNow.ToUniversalTime().ToString()}]\n\n");

                return ValueTask.CompletedTask;
            },
            BreakDuration = TimeSpan.FromSeconds(10),
            OnHalfOpened = (teste) =>
            {
                Console.WriteLine($"\n\n[CIRCUIT BREAKER SEMI-ABERTO][{DateTime.UtcNow.ToUniversalTime().ToString()}]\n\n");

                return ValueTask.CompletedTask;
            },
            OnClosed = (teste) =>
            {
                Console.WriteLine($"\n\n[CIRCUIT BREAKER FECHADO][{DateTime.UtcNow.ToUniversalTime().ToString()}]\n\n");

                return ValueTask.CompletedTask;
            },
            ShouldHandle = new PredicateBuilder().Handle<Exception>(),
            FailureRatio = 0.2,
            MinimumThroughput = 5
        };

        resiliencePipelineBuilder
            .AddTimeout(timeoutOptions)
            .AddRetry(retryOptions)
            .AddCircuitBreaker(circuitBreakerOptions);

        return resiliencePipelineBuilder.Build();
    }
}
