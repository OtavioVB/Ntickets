using Microsoft.Extensions.DependencyInjection;
using Ntickets.BuildingBlocks.ResilienceContext.Options.HttpClientRequests;
using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Polly;
using Polly.Retry;

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
