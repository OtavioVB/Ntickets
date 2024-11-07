using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;

namespace Ntickets.UnitTests.Common;

public sealed class FakerMetricManager : IMetricManager
{
    public IDictionary<string, (int, KeyValuePair<string, object?>[])> _dictionary = new Dictionary<string, (int, KeyValuePair<string, object?>[])>();

    public void CreateIfNotExistsAndIncrementCounter(string counterName = "", params KeyValuePair<string, object?>[] keyValuePairs)
    {
        if (_dictionary.ContainsKey(counterName))
        {
            _dictionary[counterName] = (_dictionary[counterName].Item1 + 1, keyValuePairs);
        }
        else
        {
            _dictionary.Add(counterName, (1, keyValuePairs));
        }
    }

    public static FakerMetricManager CreateInstance()
        => new FakerMetricManager();
}
