namespace Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;

public interface IMetricManager
{
    public void CreateIfNotExistsAndIncrementCounter(
        string counterName = "",
        params KeyValuePair<string, object?>[] keyValuePairs);
}
