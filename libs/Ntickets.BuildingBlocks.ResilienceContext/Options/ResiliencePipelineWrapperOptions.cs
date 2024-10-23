namespace Ntickets.BuildingBlocks.ResilienceContext.Options;

public class ResiliencePipelineWrapperOptions
{
    public ResiliencePipelineWrapperOptions()
    {
    }

    public ResiliencePipelineWrapperOptions(
        ResiliencePipelineTimeoutWrapperOptions timeoutOptions, 
        ResiliencePipelineCircuitBreakerWrapperOptions circuitBreakerOptions,
        ResiliencePipelineRetryWrapperOptions retryOptions)
    {
        TimeoutOptions = timeoutOptions;
        CircuitBreakerOptions = circuitBreakerOptions;
        RetryOptions = retryOptions;
    }

    public ResiliencePipelineTimeoutWrapperOptions TimeoutOptions { get; set; } = new ResiliencePipelineTimeoutWrapperOptions();
    public ResiliencePipelineCircuitBreakerWrapperOptions CircuitBreakerOptions { get; set; } = new ResiliencePipelineCircuitBreakerWrapperOptions();
    public ResiliencePipelineRetryWrapperOptions RetryOptions { get; set; } = new ResiliencePipelineRetryWrapperOptions();
}
