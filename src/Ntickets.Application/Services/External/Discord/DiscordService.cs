﻿using Microsoft.Extensions.Logging;
using Ntickets.Application.Services.External.Discord.Inputs;
using Ntickets.Application.Services.External.Discord.Interfaces;
using Ntickets.Application.Services.External.Discord.Options;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Metrics.Interfaces;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Polly;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ntickets.Application.Services.External.Discord;

public sealed class DiscordService : IDiscordService
{
    private readonly ITraceManager _traceManager;
    private readonly DiscordServiceOptions _options;
    private readonly ILogger<DiscordService> _logger;

    public DiscordService(
        ITraceManager traceManager, 
        DiscordServiceOptions options,
        ILogger<DiscordService> logger)
    {
        _traceManager = traceManager;
        _options = options;
        _logger = logger;
    }

    public Task SignalCreateTenantEventInfoOnChannelAsync(
        SignalCreateTenantEventInfoOnChannelDiscordServiceInput input,
        AuditableInfoValueObject auditableInfo,
        CancellationToken cancellationToken)
        => _traceManager.ExecuteTraceAsync(
            traceName: $"{nameof(DiscordService)}.{nameof(SignalCreateTenantEventInfoOnChannelAsync)}",
            activityKind: ActivityKind.Producer,
            input: input,
            handler: async (input, auditableInfo, activity, cancellationToken) =>
            {
                var content = $@"{{
                    ""content"": null,
                    ""embeds"": [
                        {{
                            ""title"": ""Criação de Contratante"",
                            ""description"": ""Você recebeu essa notificação, pois acabou de ocorrer a **Criação de Contratante** na plataforma, veja a seguir informações para a tomada de uma iniciativa.\n\n**ID do Contratante** {input.TenantId}\n**Nome Fantasia** {input.FantasyName}\n**Nome Legal** {input.LegalName}\n**Email** {input.Email}\n**Telefone** {input.Phone}"",
                            ""color"": 16436245,
                            ""footer"": {{
                                ""text"": ""CREATE_TENANT_EVENT {auditableInfo.GetCorrelationId()}""
                            }}
                        }}
                    ],
                    ""username"": ""Sinalizadores do Ntickets"",
                    ""attachments"": []
                }}";

                var request = new HttpRequestMessage(
                    method: HttpMethod.Post,
                    requestUri: _options.CreateTenantEventWebhookPath);

                const string REQUEST_MEDIA_TYPE = "application/json";
                request.Content = new StringContent(
                    content: content,
                    mediaType: new MediaTypeHeaderValue(
                        mediaType: REQUEST_MEDIA_TYPE));

                using var httpClient = HttpClientFactory.Create();
                httpClient.BaseAddress = new Uri(_options.Host);

                var response = await httpClient.SendAsync(
                    request: request,
                    cancellationToken: cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync(
                        cancellationToken: cancellationToken);

                    _logger.LogError(
                        message: "[{Type}][{Timestamp}][StatusCode = {StatusCodeNumber}{StatusCode}][Host = {Host}][Endpoint = {Endpoint}][Response = {Response}] The request that has sent to discord service has not got success.",
                        typeof(DiscordService),
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        (int)response.StatusCode,
                        response.StatusCode,
                        _options.Host,
                        _options.CreateTenantEventWebhookPath,
                        result);
                }
                else
                {
                    _logger.LogInformation(
                        message: "[{Type}][{Timestamp}][StatusCode = {StatusCodeNumber}{StatusCode}][Host = {Host}][Endpoint = {Endpoint}][Request = {Request}] The request that has sent to discord service has got success.",
                        typeof(DiscordService),
                        DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                        (int)response.StatusCode,
                        response.StatusCode,
                        _options.Host,
                        _options.CreateTenantEventWebhookPath,
                        content);
                }

                return Task.CompletedTask;
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
