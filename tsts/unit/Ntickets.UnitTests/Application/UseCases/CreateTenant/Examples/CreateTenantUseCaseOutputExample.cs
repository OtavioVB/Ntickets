using Ntickets.Application.UseCases.CreateTenant.Outputs;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.UnitTests.Application.UseCases.CreateTenant.Examples;

public static class CreateTenantUseCaseOutputExample
{
    public static CreateTenantUseCaseOutput Example => CreateTenantUseCaseOutput.Factory(
        tenantId: IdValueObject.Factory(),
        createdAt: DateTime.UtcNow,
        status: TenantStatusValueObject.PENDING_ANALYSIS,
        fantasyName: "Eventos",
        legalName: "Eventos LTDA",
        email: "suporte@ntickets.com.br",
        phone: "5511999999999",
        document: "00000000000000",
        lastModifiedAt: DateTime.UtcNow);
}
