using Polly;

namespace Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;

public interface IResiliencePipelineWrapper
{
    public ResiliencePipeline GetResiliencePipeline();
}
