using Microsoft.Extensions.DependencyInjection;
using Ntickets.Application.Services.TenantContext;
using Ntickets.Application.Services.TenantContext.Interfaces;
using Ntickets.Application.UseCases.Base.Interfaces;
using Ntickets.Application.UseCases.CreateTenant;
using Ntickets.Application.UseCases.CreateTenant.Inputs;
using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.Domain.BoundedContexts.EventContext.Events;

namespace Ntickets.Application;

public static class DependencyInjection
{
    public static void ApplyApplicationDependenciesConfiguration(this IServiceCollection serviceCollection)
    {
        #region Services Dependencies Configuration

        serviceCollection.AddScoped<ITenantService, TenantService>();

        #endregion

        #region Event Services Depedencies Configuration


        #endregion

        #region Use Cases Dependencies Configuration

        serviceCollection.AddScoped<IUseCase<CreateTenantUseCaseInput, CreateTenantUseCaseOutput>, CreateTenantUseCase>();

        #endregion
    }
}
