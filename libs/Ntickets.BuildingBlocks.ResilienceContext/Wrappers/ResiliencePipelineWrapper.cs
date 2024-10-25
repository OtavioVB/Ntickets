using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ntickets.BuildingBlocks.ResilienceContext.Options;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using System.Linq;

namespace Ntickets.BuildingBlocks.ResilienceContext.Wrappers;

public sealed class ResiliencePipelineWrapper : IResiliencePipelineWrapper
{
    private readonly ResiliencePipeline _resiliencePipeline;

    private ResiliencePipelineWrapper(ResiliencePipeline resiliencePipeline)
    {
        _resiliencePipeline = resiliencePipeline;
    }

    public static ResiliencePipelineWrapper Build(
        string resiliencePipelineDefinitionName,
        IServiceProvider serviceProvider,
        ResiliencePipelineWrapperOptions options)
    {
        var resiliencePipelineBuilder = new ResiliencePipelineBuilder();

        var logger = serviceProvider.GetRequiredService<ILogger<ResiliencePipelineWrapper>>();

        var retryOptions = new RetryStrategyOptions()
        {
            MaxRetryAttempts = options.RetryOptions.MaxRetryAttempts,
            Delay = options.RetryOptions.GetDelayBetweenRetries(),
            BackoffType = DelayBackoffType.Linear,
            ShouldHandle = (exception) => new ValueTask<bool>(options.CircuitBreakerOptions.HandleExceptionsCollection.Any(p => p == exception.Outcome.Exception?.GetType())),
            OnRetry = (context) => { 
                logger.LogError($"\n\nTENTATIVA {context.AttemptNumber}\n\n"); 
                return ValueTask.CompletedTask; 
            } 
        };

        var circuitBreakerOptions = new CircuitBreakerStrategyOptions()
        {
            ShouldHandle = (exception) => new ValueTask<bool>(options.CircuitBreakerOptions.HandleExceptionsCollection.Any(p => p == exception.Outcome.Exception?.GetType())),
            BreakDuration = options.CircuitBreakerOptions.GetBreakDuration(),
            FailureRatio = options.CircuitBreakerOptions.FailureRatio,
            MinimumThroughput = options.CircuitBreakerOptions.MinimumThroughput
        };

        var timeoutOptions = new TimeoutStrategyOptions()
        {
            Timeout = options.TimeoutOptions.Timeout,
            OnTimeout = (context) =>
            {
                logger.LogError(
                    message: "[{DefinitionName}][{Type}][DateTime = {DateTime}] The execution process has got timeout.",
                    resiliencePipelineDefinitionName,
                    typeof(ResiliencePipelineWrapper),
                    DateTime.UtcNow);

                return ValueTask.CompletedTask;
            }
        };

        resiliencePipelineBuilder
            .AddTimeout(timeoutOptions)
            .AddRetry(retryOptions)
            .AddCircuitBreaker(circuitBreakerOptions);

        var resiliencePipeline = resiliencePipelineBuilder.Build();

        return new ResiliencePipelineWrapper(
            resiliencePipeline: resiliencePipeline);
    }

    public ResiliencePipeline GetResiliencePipeline()
        => _resiliencePipeline;
}
