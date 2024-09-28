using Ntickets.Application.Services.Base.Inputs;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.Application.UseCases.CreateTenant.Inputs;

public readonly struct CreateTenantUseCaseInput
{
    public FantasyNameValueObject FantasyName { get; }
    public LegalNameValueObject LegalName { get; }
    public EmailValueObject Email { get; }
    public PhoneValueObject Phone { get; }
    public DocumentValueObject Document { get; }

    private CreateTenantUseCase(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone, DocumentValueObject document)
    {
        FantasyName = fantasyName;
        LegalName = legalName;
        Email = email;
        Phone = phone;
        Document = document;
    }

    public static CreateTenantUseCase Factory(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone,
        DocumentValueObject document)
        => new(fantasyName, legalName, email, phone, document);
}