using WireMock.Server;
using WireMock.Settings;

namespace Ntickets.UnitTests.Common;

public sealed class WireMockServerCustomized : IDisposable
{
    private readonly WireMockServer _wireMockServer;

    public WireMockServerCustomized(WireMockServerSettings settings)
    {
        _wireMockServer = WireMockServer.Start(settings);
    }

    public WireMockServer GetWireMockServer()
        => _wireMockServer;

    public void Dispose()
    {
        _wireMockServer.Stop();
        _wireMockServer.Dispose();
    }
}
