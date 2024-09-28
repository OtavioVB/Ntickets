using Microsoft.Extensions.DependencyInjection;
using Ntickets.Application.Services.TenantContext;
using Ntickets.Application.Services.TenantContext.Interfaces;

namespace Ntickets.Application;

public static class DependencyInjection
{
    public static void ApplyApplicationDependenciesConfiguration(this IServiceCollection serviceCollection)
    {
        #region Services Dependencies Configuration

        serviceCollection.AddScoped<ITenantService, TenantService>();

        #endregion

        #region Use Cases Dependencies Configuration



        #endregion
    }
}
