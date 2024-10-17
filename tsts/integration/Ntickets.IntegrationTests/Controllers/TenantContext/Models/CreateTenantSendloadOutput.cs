using Ntickets.IntegrationTests.Common;

namespace Ntickets.IntegrationTests.Controllers.TenantContext.Models;

internal class CreateTenantSendloadOutput
{
    public CreateTenantSendloadOutput(string tenantId, DateTime createdAt, string status, string fantasyName, string legalName, string document, string email, string phone, DateTime lastModifiedAt, Notification[] notifications)
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

    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; }
    public string FantasyName { get; set; }
    public string LegalName { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public Notification[] Notifications { get; set; }
}
