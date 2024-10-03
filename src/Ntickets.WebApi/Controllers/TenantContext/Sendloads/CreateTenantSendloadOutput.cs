using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.WebApi.Controllers.TenantContext.Sendloads;

public readonly struct CreateTenantSendloadOutput
{
    public string TenantId { get; }
    public string CreatedAt { get; }
    public string Status { get; }
    public string FantasyName { get; }
    public string LegalName { get; }
    public string Document { get; }
    public string Email { get; }
    public string Phone { get; }
    public string LastModifiedAt { get; }
    public INotification[] Notifications { get; }

    private CreateTenantSendloadOutput(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status, FantasyNameValueObject fantasyName, LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email, PhoneValueObject phone, DateTimeValueObject lastModifiedAt, INotification[] notifications)
    {
        TenantId = tenantId;
        CreatedAt = createdAt;
        Status = status;
        FantasyName = fantasyName;
        LegalName = legalName;
        Document = document;
        Email = email;
        Phone = phone;
        LastModifiedAt = lastModifiedAt;
        Notifications = notifications;
    }

    public static CreateTenantSendloadOutput Factory(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status, FantasyNameValueObject fantasyName,
        LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email, PhoneValueObject phone, DateTimeValueObject lastModifiedAt,
        INotification[] notifications)
        => new(tenantId, createdAt, status, fantasyName, legalName, document, email, phone, lastModifiedAt, notifications);
}
