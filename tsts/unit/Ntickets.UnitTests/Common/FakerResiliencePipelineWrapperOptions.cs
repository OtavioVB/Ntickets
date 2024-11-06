using Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

namespace Ntickets.UnitTests.Common;

public static class FakerResiliencePipelineWrapperOptions
{
    public static ResiliencePipelineWrapperOptions CreateInstance()
        => new ResiliencePipelineWrapperOptions()
        {
            CircuitBreakerOptions = new ResiliencePipelineCircuitBreakerWrapperOptions()
            {
                BreakDurationInSeconds = 10,
                HandleExceptionsCollection = ["SocketException", "PostgresException", "NpgsqlException", "TimeoutException", "InvalidOperationException"],
                FailureRatio = 0.1,
                MinimumThroughput = 50,
            },
            RetryOptions = new ResiliencePipelineRetryWrapperOptions()
            {
                DelayBetweenRetriesInMiliseconds = 100,
                MaxRetryAttempts = 5,
                HandleExceptionsCollection = ["SocketException", "PostgresException", "NpgsqlException", "TimeoutException", "InvalidOperationException"]
            },
            TimeoutOptions = new ResiliencePipelineTimeoutWrapperOptions()
            {
                TimeoutInSeconds = 10,
            }
        };
}
