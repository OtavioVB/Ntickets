namespace Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

public class ResiliencePipelineTimeoutWrapperOptions
{
    public ResiliencePipelineTimeoutWrapperOptions()
    {
    }

    public ResiliencePipelineTimeoutWrapperOptions(int timeout)
    {
        TimeoutInSeconds = timeout;
    }

    public int TimeoutInSeconds { get; set; }

    public TimeSpan GetTimeSpan()
        => TimeSpan.FromSeconds(TimeoutInSeconds);
}
