using Ntickets.Domain.BoundedContexts.EventContext.Events;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;

namespace Ntickets.UnitTests.Domain.BoundedContexts.TenantContext.Events;

public sealed class CreateTenantEventValidationTests
{
    [Fact]
    public void Given_Create_Tenant_Event_Input_Should_Be_Equal_Properties()
    {
        // Arrange
        var tenantId = Guid.NewGuid().ToString();
        var dateTime = DateTime.UtcNow;
        var fantasyName = Guid.NewGuid().ToString();
        var legalName = Guid.NewGuid().ToString();
        var document = "00000000000";
        var email = $"{Guid.NewGuid().ToString()}@ntickets.com.br";
        var phone = "5511999999999";
        var status = EnumTenantStatus.APPROVED;

        // Act
        var tenantEvent = new CreateTenantEvent(
            tenantId: tenantId,
            createdAt: dateTime,
            fantasyName: fantasyName,
            legalName: legalName,
            document: document,
            email: email,
            phone: phone,
            status: EnumTenantStatus.APPROVED,
            lastModifiedAt: dateTime);

        // Assert
        Assert.Equal(tenantId, tenantEvent.TenantId);
        Assert.Equal(dateTime, tenantEvent.CreatedAt);
        Assert.Equal(fantasyName, tenantEvent.FantasyName);
        Assert.Equal(legalName, tenantEvent.LegalName);
        Assert.Equal(document, tenantEvent.Document);
        Assert.Equal(phone, tenantEvent.Phone);
        Assert.Equal(email, tenantEvent.Email);
        Assert.Equal(status, tenantEvent.Status);
        Assert.Equal(dateTime, tenantEvent.LastModifiedAt);
    }
}
