using Ntickets.BuildingBlocks.ObservabilityContext.Traces;

namespace Ntickets.UnitTests.Common;

public sealed class FakerTraceManager
{
    public static TraceManager CreateInstance()
        => new TraceManager(FakerActivitySourceWrapper.CreateInstance());
}
