using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Wrappers.Interfaces;
using System.Diagnostics;

namespace Ntickets.UnitTests.Common;

public sealed class FakerActivitySourceWrapper : IActivitySourceWrapper
{
    public Activity? StartActivity(string name = "", ActivityKind kind = ActivityKind.Internal)
        => FakerActivity.CreateInstance();

    public static FakerActivitySourceWrapper CreateInstance()
        => new FakerActivitySourceWrapper();    
}
