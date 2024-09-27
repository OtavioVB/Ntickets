using Ntickets.Domain.ValueObjects;

namespace Ntickets.Application.Services.TenantContext.Outputs;

public readonly struct CreateTenantServiceOutput
{
    public IdValueObject TenantId { get; }
    public DateTimeValueObject CreatedAt { get; }
    public TenantStatusValueObject Status { get; }
    public FantasyNameValueObject FantasyName { get; }
    public LegalNameValueObject LegalName { get; }
    public DocumentValueObject Document { get; }
    public EmailValueObject Email { get; }
    public PhoneValueObject Phone { get; }
    public DateTimeValueObject LastModifiedAt { get; }

    private CreateTenantServiceOutput(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status, 
        FantasyNameValueObject fantasyName, LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email, 
        PhoneValueObject phone, DateTimeValueObject lastModifiedAt)
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
    }

    public static CreateTenantServiceOutput Factory(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status,
        FantasyNameValueObject fantasyName, LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email,
        PhoneValueObject phone, DateTimeValueObject lastModifiedAt)
        => new(tenantId, createdAt, status, fantasyName, legalName, document, email, phone, lastModifiedAt);
}
