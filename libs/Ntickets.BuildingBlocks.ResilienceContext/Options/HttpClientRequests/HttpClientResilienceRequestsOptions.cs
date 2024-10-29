using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

namespace Ntickets.BuildingBlocks.ResilienceContext.Options.HttpClientRequests;

public class HttpClientResilienceRequestsOptions
{
    public string Host { get; set; } = string.Empty;
    public string PipelineName { get; set; } = string.Empty;
    public ResiliencePipelineRetryWrapperOptions RetryOptions { get; set; } = new ResiliencePipelineRetryWrapperOptions();
}
