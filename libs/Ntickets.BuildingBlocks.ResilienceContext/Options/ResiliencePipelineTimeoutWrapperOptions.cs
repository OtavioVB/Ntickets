namespace Ntickets.BuildingBlocks.ResilienceContext.Options;

public class ResiliencePipelineTimeoutWrapperOptions
{
    public ResiliencePipelineTimeoutWrapperOptions()
    {
    }

    public ResiliencePipelineTimeoutWrapperOptions(TimeSpan timeout)
    {
        Timeout = timeout;
    }

    public TimeSpan Timeout { get; set; }
}
