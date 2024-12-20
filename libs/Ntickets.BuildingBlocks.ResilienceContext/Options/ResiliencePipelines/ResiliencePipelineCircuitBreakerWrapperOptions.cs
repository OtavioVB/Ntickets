﻿using System.Collections.Immutable;

namespace Ntickets.BuildingBlocks.ResilienceContext.Options.ResiliencePipelines;

public class ResiliencePipelineCircuitBreakerWrapperOptions
{
    public int BreakDurationInSeconds { get; set; }
    public string[] HandleExceptionsCollection { get; set; } = [];
    public double FailureRatio { get; set; }
    public int MinimumThroughput { get; set; }

    public ResiliencePipelineCircuitBreakerWrapperOptions()
    {
    }

    public ResiliencePipelineCircuitBreakerWrapperOptions(int breakDurationInSeconds, string[] handleExceptionsCollection, double failureRatio, int minimumThroughput)
    {
        BreakDurationInSeconds = breakDurationInSeconds;
        HandleExceptionsCollection = handleExceptionsCollection;
        FailureRatio = failureRatio;
        MinimumThroughput = minimumThroughput;
    }

    public TimeSpan GetBreakDuration()
        => TimeSpan.FromSeconds(BreakDurationInSeconds);
}
