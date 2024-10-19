using System.Diagnostics;

namespace Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers.Interfaces;

public interface IActivitySourceWrapper
{
    public Activity? StartActivity(string name = "", ActivityKind kind = ActivityKind.Internal);
}
