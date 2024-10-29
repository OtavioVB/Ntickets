using System.Collections.Immutable;

namespace Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

public class ResiliencePipelineRetryWrapperOptions
{
    public string[] HandleExceptionsCollection { get; set; } = [];
    public int MaxRetryAttempts { get; set; }
    public int DelayBetweenRetriesInMiliseconds { get; set; }

    public ResiliencePipelineRetryWrapperOptions()
    {
    }

    public ResiliencePipelineRetryWrapperOptions(string[] handleExceptionsCollection, int maxRetryAttempts, int delayBetweenRetriesInSeconds)
    {
        HandleExceptionsCollection = handleExceptionsCollection;
        MaxRetryAttempts = maxRetryAttempts;
        DelayBetweenRetriesInMiliseconds = delayBetweenRetriesInSeconds;
    }

    public TimeSpan GetDelayBetweenRetries()
        => TimeSpan.FromMilliseconds(DelayBetweenRetriesInMiliseconds);
}
