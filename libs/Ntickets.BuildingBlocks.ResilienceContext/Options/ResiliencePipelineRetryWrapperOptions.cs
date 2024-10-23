using System.Collections.Immutable;

namespace Ntickets.BuildingBlocks.ResilienceContext.Options;

public class ResiliencePipelineRetryWrapperOptions
{
    public ImmutableArray<Type> HandleExceptionsCollection { get; set; }
    public int MaxRetryAttempts { get; set; }
    public int DelayBetweenRetriesInMiliseconds { get; set; }

    public ResiliencePipelineRetryWrapperOptions()
    {
    }

    public ResiliencePipelineRetryWrapperOptions(ImmutableArray<Type> handleExceptionsCollection, int maxRetryAttempts, int delayBetweenRetriesInSeconds)
    {
        HandleExceptionsCollection = handleExceptionsCollection;
        MaxRetryAttempts = maxRetryAttempts;
        DelayBetweenRetriesInMiliseconds = delayBetweenRetriesInSeconds;
    }

    public TimeSpan GetDelayBetweenRetries()
        => TimeSpan.FromSeconds(DelayBetweenRetriesInMiliseconds);  
}
