using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Ntickets.Application.Services.External.Discord;
using Ntickets.Application.Services.External.Discord.Inputs;
using Ntickets.Application.Services.External.Discord.Options;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.UnitTests.Common;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Settings;

namespace Ntickets.UnitTests.Application.Services.External.DiscordContext;

public sealed class DiscordServiceValidationTests
{
    private readonly Fixture _fixture = new Fixture();

    [Fact]
    public async Task Given_Not_Valid_Discord_Request_Should_Register_Log_Error_About_The_Request()
    {
        // Arrange
        var initializer = new WireMockServerCustomized(
            settings: new WireMockServerSettings());
        using var server = initializer.GetWireMockServer();

        server
            .Given(Request.Create().WithPath("/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(StatusCodes.Status503ServiceUnavailable));

        var loggerMock = new Mock<ILogger<DiscordService>>();

        var discordService = new DiscordService(
            traceManager: FakerTraceManager.CreateInstance(),
            options: new DiscordServiceOptions()
            {
                CreateTenantEventWebhookPath = "/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP",
                Host = server.Urls[0]
            },
            logger: loggerMock.Object);

        // Act
        await discordService.SignalCreateTenantEventInfoOnChannelAsync(
            input: _fixture.Create<SignalCreateTenantEventInfoOnChannelDiscordServiceInput>(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(level => level == LogLevel.Error),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The request that has sent to discord service has not got success.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }

    [Fact]
    public async Task Given_Valid_Discord_Request_Should_Register_Log_Information_About_The_Request()
    {
        // Arrange
        var initializer = new WireMockServerCustomized(
            settings: new WireMockServerSettings());
        using var server = initializer.GetWireMockServer();

        server
            .Given(Request.Create().WithPath("/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(StatusCodes.Status204NoContent));

        var loggerMock = new Mock<ILogger<DiscordService>>();

        var discordService = new DiscordService(
            traceManager: FakerTraceManager.CreateInstance(),
            options: new DiscordServiceOptions()
            {
                CreateTenantEventWebhookPath = "/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP",
                Host = server.Urls[0]
            },
            logger: loggerMock.Object);

        // Act
        await discordService.SignalCreateTenantEventInfoOnChannelAsync(
            input: _fixture.Create<SignalCreateTenantEventInfoOnChannelDiscordServiceInput>(),
            auditableInfo: AuditableInfoValueObject.Factory(Guid.NewGuid().ToString()),
            cancellationToken: CancellationToken.None);

        // Assert
        loggerMock.Verify(
            x => x.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("The request that has sent to discord service has got success.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()!),
            Times.Once);
    }
}
