using Ntickets.Application.Services.Internal.TenantContext.Outputs;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.UnitTests.Application.Services.Internal.TenantContext.Generators;

public static class CreateTenantServiceOutputGenerator
{
    public static CreateTenantServiceOutput Generate()
        => CreateTenantServiceOutput.Factory(
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
