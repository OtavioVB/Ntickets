using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Infrascructure.EntityFrameworkCore;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Base.Interfaces;
using Ntickets.Infrascructure.EntityFrameworkCore.Repositories.Extensions;

namespace Ntickets.Infrascructure;

public static class DependencyInjection
{
    public static void ApplyInfrascructureDependenciesConfiguration(
        this IServiceCollection serviceCollection,
        string connectionString)
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

        #region Entity Framework Core Repositories Configuration

        serviceCollection.AddScoped<IBaseRepository<Tenant>, TenantRepository>();
        serviceCollection.AddScoped<IExtensionTenantRepository, TenantRepository>();

        #endregion
    }
}
