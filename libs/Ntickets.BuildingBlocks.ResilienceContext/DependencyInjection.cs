using Microsoft.Extensions.DependencyInjection;
using Ntickets.BuildingBlocks.ResilienceContext.Options;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;

namespace Ntickets.BuildingBlocks.ResilienceContext;

public static class DependencyInjection
{
    public static IServiceCollection AddKeyedResiliencePipelineWrapper(
        this IServiceCollection serviceCollection,
        string definitionName,
        Action<ResiliencePipelineWrapperOptions> optionsAction)
    {
        var options = new ResiliencePipelineWrapperOptions();

        optionsAction(options);

        serviceCollection.AddKeyedSingleton<IResiliencePipelineWrapper, ResiliencePipelineWrapper>(
            serviceKey: definitionName, 
            implementationFactory: (serviceProvider, serviceKey)
                => ResiliencePipelineWrapper.Build(
                    resiliencePipelineDefinitionName: definitionName,
                    serviceProvider: serviceProvider,
                    options: options));

        return serviceCollection;
    }

    public static IServiceCollection AddKeyedResiliencePipelineWrapper(
        this IServiceCollection serviceCollection,
        string definitionName,
        ResiliencePipelineWrapperOptions options)
    {
        serviceCollection.AddKeyedSingleton<IResiliencePipelineWrapper, ResiliencePipelineWrapper>(
            serviceKey: definitionName,
            implementationFactory: (serviceProvider, serviceKey)
                => ResiliencePipelineWrapper.Build(
                    resiliencePipelineDefinitionName: definitionName,
                    serviceProvider: serviceProvider,
                    options: options));

        return serviceCollection;
    }
}
