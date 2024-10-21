using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using System.Diagnostics.Metrics;

namespace Ntickets.BuildingBlocks.ObservabilityContext.Metrics;

public sealed class MetricManager : IMetricManager
{
    private readonly Meter _meter;
    private readonly IDictionary<string, Counter<int>> _countersDictionary;

    public MetricManager(Meter meter)
    {
        _meter = meter;
        _countersDictionary = new Dictionary<string, Counter<int>>();
    }

    public void CreateIfNotExistsAndIncrementCounter(
        string counterName = "",
        params KeyValuePair<string, object?>[] keyValuePairs)
    {
        const int INCREMENT_COUNT = 1;

        if (_countersDictionary.ContainsKey(counterName))
        {
            var counter = _countersDictionary[counterName];
            counter.Add(INCREMENT_COUNT, keyValuePairs);
        }
        else
        {
            var createdCounter = _meter.CreateCounter<int>(
                name: counterName);
            _countersDictionary.Add(
                key: counterName,
                value: createdCounter);
            var counter = _countersDictionary[counterName];
            counter.Add(INCREMENT_COUNT, keyValuePairs);
        }
    }
}
