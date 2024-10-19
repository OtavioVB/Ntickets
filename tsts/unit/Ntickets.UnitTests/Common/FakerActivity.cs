using System.Diagnostics;

namespace Ntickets.UnitTests.Common;

public sealed class FakerActivity : Activity
{
    private FakerActivity() : base("tests")
    {
    }

    public static FakerActivity CreateInstance()
        => new FakerActivity();
}
