using Ntickets.FunctionalTests.UseCases.CreateTenant.Models;
using System.Net.Http.Json;
using System.Net;
using System.Text.Json;

namespace Ntickets.FunctionalTests.UseCases.CreateTenant;

[Binding]
public class CreateTenantStepDefinitions
{
    private string _createTenantUrl = "http://localhost:5001/api/v1/business-intelligence/tenants";
    private HttpClient _httpClient = new HttpClient();

    private CreateTenantPayloadInput? _createTenantPayloadInput = null;
    private CreateTenantSendloadOutput? _createTenantSendloadOutput = null;
    private HttpResponseMessage? _httpResponseMessage = null;

    [Given(@"um usuário anônimo da plataforma com informações válidas de (.*), (.*), (.*), (.*) e (.*)")]
    public void DadoUmUsuarioAnonimoQueGostariaCriarContracaoDoServicoNaPlataformaComDadosValidos(
        string fantasyName, string legalName, string document, string email, string phone)
    {
        var payload = new CreateTenantPayloadInput(
            fantasyName: fantasyName,
            legalName: legalName,
            document: document,
            email: email,
            phone: phone);

        _createTenantPayloadInput = payload;
    }

    [When(@"enviar a solicitação de criação da contratação")]
    public async Task QuandoForSolicitadoACriacaoDaContratacaoDoServicoNaPlataforma()
    {
        _httpClient.DefaultRequestHeaders.Add(
            name: "X-Correlation-Id",
            value: Guid.NewGuid().ToString());

        var response = await _httpClient.PostAsJsonAsync(
            requestUri: _createTenantUrl,
            value: _createTenantPayloadInput,
            cancellationToken: CancellationToken.None);

        var responseString = await response.Content.ReadAsStringAsync();

        _httpResponseMessage = response;

        response.IsSuccessStatusCode.Should().BeTrue();

        _createTenantSendloadOutput = JsonSerializer.Deserialize<CreateTenantSendloadOutput>(responseString, options: new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
    }

    [Then(@"a criação do contratante deve ser realizado com sucesso")]
    public async Task EntaoACriacaoDoContratanteDeveSerRealizadoComSucesso()
    {
        _httpResponseMessage!.StatusCode.Should().Be(HttpStatusCode.Created);

        (await _httpResponseMessage!.Content.ReadAsStringAsync()).Should().NotBeNullOrEmpty();
    }

    [Then(@"o status de utilização deve constar como PENDING_ANALYSIS")]
    public void E_EntaoStatusDeUtilizacaoDaContratanteDeveConstarComoPendenteDeAnalise()
    {
        const string expectedStatus = "PENDING_ANALYSIS";

        _createTenantSendloadOutput!.Status.Should().Be(expectedStatus);
    }

    [Then(@"a mensagem de notificação de sucesso 'A criação do whitelabel foi executada com sucesso.' com código 'CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL' deve ser retornada")]
    public void E_EntaoAMensagemDeNotificacaoDeSucessoDeveSerRetornada()
    {
        const string expectedMessageCode = "CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL";
        const string expectedMessage = "A criação do whitelabel foi executada com sucesso.";
        const int expectedMessagesCount = 1;

        _createTenantSendloadOutput!.Notifications.Should().HaveCount(expectedMessagesCount);
        _createTenantSendloadOutput.Notifications[0].Code.Should().Be(expectedMessageCode);
        _createTenantSendloadOutput.Notifications[0].Message.Should().Be(expectedMessage);
        _createTenantSendloadOutput.Notifications[0].Type.Should().Be("Success");
    }
}
