using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;

public sealed record Tenant
{
    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FantasyName { get; set; }
    public string LegalName { get; set; }
    public string Document { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public EnumTenantStatus Status { get; set; }
    public DateTime LastModifiedAt { get; set; }

    public Tenant(string tenantId, DateTime createdAt, string fantasyName, string legalName, string document, string email, 
        string phone, EnumTenantStatus status, DateTime lastModifiedAt)
    {
        TenantId = tenantId;
        CreatedAt = createdAt;
        FantasyName = fantasyName;
        LegalName = legalName;
        Document = document;
        Email = email;
        Phone = phone;
        Status = status;
        LastModifiedAt = lastModifiedAt;
    }

    public static Tenant Create(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email,
        PhoneValueObject phone)
        => new(IdValueObject.Factory(), DateTimeValueObject.Factory(), fantasyName, legalName, document, email, phone, TenantStatusValueObject.PENDING_ANALYSIS, DateTimeValueObject.Factory());
}
