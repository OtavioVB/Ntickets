using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers.Interfaces;
using System.Diagnostics;

namespace Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers;

public class ActivitySourceWrapper : IActivitySourceWrapper
{
    private readonly ActivitySource _activitySource;

    public ActivitySourceWrapper(string serviceName, string serviceVersion)
    {
        _activitySource = new ActivitySource(
            name: serviceName,
            version: serviceVersion);
    }

    public virtual Activity? StartActivity(string name = "", ActivityKind kind = ActivityKind.Internal)
        => _activitySource.StartActivity(name, kind);
}
