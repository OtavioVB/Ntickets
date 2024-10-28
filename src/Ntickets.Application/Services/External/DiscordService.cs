using Ntickets.Application.Services.External.Inputs;
using Ntickets.Application.Services.External.Interfaces;
using Ntickets.BuildingBlocks.AuditableInfoContext;
using Ntickets.BuildingBlocks.ObservabilityContext.Traces.Interfaces;
using Ntickets.BuildingBlocks.ResilienceContext.Wrappers.Interfaces;
using Polly;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ntickets.Application.Services.External;

public sealed class DiscordService : IDiscordService
{
    private readonly ITraceManager _traceManager;
    private readonly HttpClient _httpClient;


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
                const string REQUEST_MEDIA_TYPE = "application/json";

                const string ENDPOINT = "https://discord.com/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP";

                using var httpClient = HttpClientFactory.Create();

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

                var response = await httpClient.PostAsync(
                            requestUri: ENDPOINT,
                            content: new StringContent(
                                content: content,
                                mediaType: new MediaTypeHeaderValue(
                                    mediaType: REQUEST_MEDIA_TYPE)),
                            cancellationToken: cancellationToken);

                return Outcome.FromResultAsValueTask(response);
            },
            auditableInfo: auditableInfo,
            cancellationToken: cancellationToken,
            keyValuePairs: []);
}
