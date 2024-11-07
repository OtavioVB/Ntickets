using Ntickets.Application.Services.Internal.TenantContext.Inputs;

namespace Ntickets.UnitTests.Application.Services.Internal.TenantContext.Generators;

public static class CreateTenantServiceInputGenerator
{
    public static CreateTenantServiceInput Generate()
        => CreateTenantServiceInput.Factory(
            fantasyName: Guid.NewGuid().ToString(),
            legalName: Guid.NewGuid().ToString(),
            email: $"{Guid.NewGuid().ToString()}@ntickets.com.br",
            phone: $"{new Random().NextInt64(1000000000000, 9999999999999)}",
            document: $"{new Random().NextInt64(10000000000, 99999999999)}");

    public static CreateTenantServiceInput GenerateInvalid()
        => CreateTenantServiceInput.Factory(
            fantasyName: "",
            legalName: Guid.NewGuid().ToString(),
            email: $"{Guid.NewGuid().ToString()}@ntickets.com.br",
            phone: $"{new Random().NextInt64(1000000000000, 9999999999999)}",
            document: $"{new Random().NextInt64(10000000000, 99999999999)}");
}
