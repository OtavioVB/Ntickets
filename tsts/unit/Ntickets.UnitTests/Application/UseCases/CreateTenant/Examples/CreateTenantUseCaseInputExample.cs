using Ntickets.Application.UseCases.CreateTenant.Inputs;

namespace Ntickets.UnitTests.Application.UseCases.CreateTenant.Examples;

public static class CreateTenantUseCaseInputExample
{
    public static CreateTenantUseCaseInput Example => CreateTenantUseCaseInput.Factory(
        fantasyName: "Eventos",
        legalName: "Eventos LTDA",
        email: "suporte@ntickets.com.br",
        phone: "5511999999999",
        document: "00000000000000");
}
