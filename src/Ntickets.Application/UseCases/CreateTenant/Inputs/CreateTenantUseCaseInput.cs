using Ntickets.Domain.ValueObjects;

namespace Ntickets.Application.UseCases.CreateTenant.Inputs;

public readonly struct CreateTenantUseCaseInput
{
    public FantasyNameValueObject FantasyName { get; }
    public LegalNameValueObject LegalName { get; }
    public EmailValueObject Email { get; }
    public PhoneValueObject Phone { get; }
    public DocumentValueObject Document { get; }

    private CreateTenantUseCaseInput(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone, DocumentValueObject document)
    {
        FantasyName = fantasyName;
        LegalName = legalName;
        Email = email;
        Phone = phone;
        Document = document;
    }

    public static CreateTenantUseCaseInput Factory(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone,
        DocumentValueObject document)
        => new(fantasyName, legalName, email, phone, document);
}