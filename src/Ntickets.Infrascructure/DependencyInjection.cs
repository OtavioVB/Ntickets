using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;

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

        serviceCollection.AddScoped<IBaseRepository<Tenant>, TenantRepository>();
        serviceCollection.AddScoped<IExtensionTenantRepository, TenantRepository>();

        #endregion

        #region RabbitMq Connection Configuration

        #endregion
    }
}
