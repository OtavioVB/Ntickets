using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;

namespace Ntickets.UnitTests.Domain.BoundedContexts.TenantContext.Events.Faker;

public static class FakerCreateTenantEvent
{
    public static CreateTenantEvent CreateInstance()
        => new CreateTenantEvent(
            tenantId: Ulid.NewUlid().ToString(),
            createdAt: DateTime.UtcNow,
            fantasyName: "Eventos",
            legalName: "Eventos LTDA",
            document: "00000000000100",
            email: "eventos@ntickets.com.br",
            phone: "5511999999999",
            status: EnumTenantStatus.PENDING_ANALYSIS,
            lastModifiedAt: DateTime.UtcNow,
            eventName: "CREATE_TENANT_EVENT",
            correlationId: Guid.NewGuid().ToString());
}
