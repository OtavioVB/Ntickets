using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;

namespace Ntickets.UnitTests.Common;

public sealed class FakerMetricManager : IMetricManager
{
    public void CreateIfNotExistsAndIncrementCounter(string counterName = "", params KeyValuePair<string, object?>[] keyValuePairs)
        => Console.WriteLine(string.Empty);

    public static IMetricManager CreateInstance()
        => new FakerMetricManager();
}
