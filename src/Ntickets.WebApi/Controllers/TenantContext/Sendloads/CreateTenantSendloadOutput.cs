using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.WebApi.Controllers.TenantContext.Sendloads;

public struct CreateTenantSendloadOutput
{
    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string FantasyName { get; set; }
    public string LegalName { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public INotification[] Notifications { get; set; }

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
