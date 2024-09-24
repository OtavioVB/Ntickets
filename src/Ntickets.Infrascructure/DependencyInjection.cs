using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ntickets.Infrascructure.EntityFrameworkCore;

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
    }
}
