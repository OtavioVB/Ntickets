using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Ntickets.IntegrationTests.Common;
using Ntickets.IntegrationTests.Controllers.TenantContext.Models;
using Ntickets.WebApi.Controllers.TenantContext.Payloads;
using System.Net;
using System.Net.Http.Json;

namespace Ntickets.IntegrationTests.Controllers.TenantContext;

public sealed class CreateTenantIntegratedTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _customWebApplicationFactory;

    public CreateTenantIntegratedTests(CustomWebApplicationFactory factory)
    {
        _customWebApplicationFactory = factory;   
    }

    [Theory]
    [InlineData("Eventos", "Eventos LTDA", "21330277000152", "eventos-pj@ntickets.com.br", "5511959937833")]
    [InlineData("Eventos", "Otavio Pessoa Fisica", "65360341041", "eventos-pf@ntickets.com.br", "5511958520009")]
    public async Task Create_Tenant_Successfull_When_Anonymous_User_Send_Valid_Tenant_Info(
        string fantasyName, string legalName, string document, string email, string phone)
    {
        const string EXPECTED_STATUS = "PENDING_ANALYSIS";
        const string EXPECTED_SUCCESS_NOTIFICATION_CODE = "CREATE_TENANT_HAS_BEEN_EXECUTED_SUCCESSFULL";
        const string EXPECTED_SUCCESS_NOTIFICATION_MESSAGE = "A criação do whitelabel foi executada com sucesso.";
        const string EXPECTED_SUCCESS_NOTIFICATION_TYPE = "Success";

        // Arrange
        const string ENDPOINT = "/api/v1/business-intelligence/tenants";
        
        using var httpClient = _customWebApplicationFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add(
            name: "X-Correlation-Id",
            value: Guid.NewGuid().ToString());

        using var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: ENDPOINT);

        request.Content = JsonContent.Create(
            inputValue: new CreateTenantPayloadInput(
                fantasyName: fantasyName,
                legalName: legalName,
                document: document,
                email: email,
                phone: phone));

        // Act
        var httpResult = await httpClient.SendAsync(
            request: request,
            cancellationToken: CancellationToken.None);

        var content = await httpResult.Content.ReadFromJsonAsync<CreateTenantSendloadOutput>();

        // Assert
        httpResult.StatusCode.Should().Be(HttpStatusCode.Created);

        content.Should().NotBeNull();

        content!.TenantId.Should().NotBeNullOrWhiteSpace();
        content.TenantId.Length.Should().Be(Ulid.Empty.ToString().Length);

        content.CreatedAt.Should().NotBeAfter(DateTime.UtcNow);

        content.Status.Should().Be(EXPECTED_STATUS);

        content.FantasyName.Should().NotBeNullOrWhiteSpace();

        content.LegalName.Should().NotBeNullOrWhiteSpace();

        content.Document.Should().Be(document);

        content.Email.Should().Be(email.ToLower());

        content.Phone.Should().Be(phone);

        content.LastModifiedAt.Should().NotBeAfter(DateTime.UtcNow);

        content.Notifications.Should().ContainSingle();
        content.Notifications[0].Code.Should().Be(EXPECTED_SUCCESS_NOTIFICATION_CODE);
        content.Notifications[0].Message.Should().Be(EXPECTED_SUCCESS_NOTIFICATION_MESSAGE);
        content.Notifications[0].Type.Should().Be(EXPECTED_SUCCESS_NOTIFICATION_TYPE);
    }

    [Theory]
    [InlineData("Eventos", "Eventos LTDA", "21330277000152", "eventos-pj@ntickets.com.br", "551195852344")]
    [InlineData("Eventos", "Eventos LTDA", "21330277000152", "eventokets.com.br", "5511958523443")]
    [InlineData("Eventos", "", "21330277000152", "eventos-pj@ntickets.com.br", "5511958523443")]
    [InlineData("", "Eventos LTDA", "21330277000152", "eventos-pj@ntickets.com.br", "5511958523443")]
    [InlineData("Eventos", "Eventos LTDA", "21302XX000152", "eventos-pj@ntickets.com.br", "5511958523443")]
    public async Task Create_Tenant_Must_Be_Return_Error_Response_When_Any_Data_Is_Not_Valid(
        string fantasyName, string legalName, string document, string email, string phone)
    {
        // Arrange
        const string ENDPOINT = "/api/v1/business-intelligence/tenants";

        using var httpClient = _customWebApplicationFactory.CreateClient();

        httpClient.DefaultRequestHeaders.Add(
            name: "X-Correlation-Id",
            value: Guid.NewGuid().ToString());

        using var request = new HttpRequestMessage(
            method: HttpMethod.Post,
            requestUri: ENDPOINT);

        request.Content = JsonContent.Create(
            inputValue: new CreateTenantPayloadInput(
                fantasyName: fantasyName,
                legalName: legalName,
                document: document,
                email: email,
                phone: phone));

        // Act
        var httpResult = await httpClient.SendAsync(
            request: request,
            cancellationToken: CancellationToken.None);

        var content = await httpResult.Content.ReadFromJsonAsync<Notification[]>();

        // Assert
        httpResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        content.Should().NotBeNull();

        content!.Should().NotBeEmpty();
    }
}
