using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork;
using Ntickets.Infrascructure.EntityFrameworkCore.UnitOfWork.Interfaces;
using Ntickets.Infrascructure.RabbitMq;
using Ntickets.Infrascructure.RabbitMq.Interfaces;

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
        string rabbitMqConnectionClientProviderName)
    {
        #region Entity Framework Core DbContext Configuration

        const int DEFAULT_DB_CONTEXT_POOL_SIZE = 512;
        const string DB_CONTEXT_ASSEMBLY_MIGRATIONS = "Ntickets.Infrascructure";

        serviceCollection.AddDbContextPool<DataContext>(
            optionsAction: options => options.UseNpgsql(
                connectionString: connectionString,
                npgsqlOptionsAction: npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                    assemblyName: DB_CONTEXT_ASSEMBLY_MIGRATIONS)),
            poolSize: DEFAULT_DB_CONTEXT_POOL_SIZE);

        #endregion

        #region Entity Framework Core UnitOfWork Configuration

        serviceCollection.AddScoped<IUnitOfWork, DefaultUnitOfWork>();

        #endregion

        #region Entity Framework Core Repositories Configuration

        serviceCollection.AddScoped<IBaseRepository<Tenant>, TenantRepository>();
        serviceCollection.AddScoped<IExtensionTenantRepository, TenantRepository>();

        #endregion

        #region RabbitMq Connection Configuration

        serviceCollection.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>((serviceProvider)
            => new RabbitMqPublisher(
                rabbitMqConnectionUserName: rabbitMqConnectionUserName,
                rabbitMqConnectionPassword: rabbitMqConnectionPassword,
                rabbitMqConnectionVirtualHost: rabbitMqConnectionVirtualHost,
                rabbitMqConnectionHostName: rabbitMqConnectionHostName,
                rabbitMqConnectionClientProviderName: rabbitMqConnectionClientProviderName));

        #endregion
    }
}
