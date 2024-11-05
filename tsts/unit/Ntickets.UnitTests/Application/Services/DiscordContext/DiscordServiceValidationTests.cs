using Microsoft.AspNetCore.Http;
using Moq;
using Ntickets.Application.Services.External.Discord;
using Ntickets.Application.Services.External.Discord.Options;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.UnitTests.Common;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace Ntickets.UnitTests.Application.Services.DiscordContext;

public sealed class DiscordServiceValidationTests
{
    [Fact]
    public async Task Given_Valid_Create_Tenant_Event_Should_Send_Request_To_Discord()
    {
        // Arrange
        var initializer = new WireMockServerCustomized(
            settings: new WireMockServerSettings());
        var server = initializer.GetWireMockServer();

        server
            .Given(Request.Create().WithPath("/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP").UsingPost())
            .RespondWith(Response.Create().WithProxy("https://discord.com").WithStatusCode(StatusCodes.Status204NoContent));

        var discordService = new DiscordService(
            metricManager: FakerMetricManager.CreateInstance(),
            traceManager: FakerTraceManager.CreateInstance(),
            options: new DiscordServiceOptions()
            {
                CreateTenantEventWebhookPath = "/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP",
                Host = "https://discord.com"
            });

        // Act
        var discordServiceResult = await discordService();

        // Assert
    }
}
