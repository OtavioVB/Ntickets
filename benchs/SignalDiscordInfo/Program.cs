using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading;

namespace SignalDiscordInfo;

public sealed class Program
{
    static async Task Main(string[] args)
    {
        const string ENDPOINT = "https://discord.com/api/webhooks/1299499327680675900/isllUQKXpOPY_O_I5L0Hli4_CYranxiFUPJ3xaIxX0m7NmITTr9vBZV2zLyjzlp2h7mP";

        using var httpClient = HttpClientFactory.Create();

        var content = $@"{{
            ""content"": null,
            ""embeds"": [
            {{
                ""title"": ""Criação de Contratante"",
                ""description"": ""Você recebeu essa notificação, pois acabou de ocorrer a **Criação de Contratante** na plataforma, veja a seguir informações para a tomada de uma iniciativa.\n\n**ID do Contratante** {0}\n**Nome Fantasia** {1}\n**Nome Legal** {2}\n**Email** {3}\n**Telefone** {4}"",
                ""color"": 16436245,
                ""footer"": {{
                ""text"": ""CREATE_TENANT_EVENT {5}""
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
                encoding: Encoding.UTF8,
                mediaType: new MediaTypeHeaderValue("application/json")),
            cancellationToken: CancellationToken.None);

        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine();
    }
}
