using Ntickets.Application.Services.Internal.Base.Inputs;
using Ntickets.BuildingBlocks.MethodResultsContext;
using Ntickets.BuildingBlocks.NotificationContext.Interfaces;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.Application.Services.Internal.TenantContext.Inputs;

public readonly struct CreateTenantServiceInput : IServiceInput
{
    public FantasyNameValueObject FantasyName { get; }
    public LegalNameValueObject LegalName { get; }
    public EmailValueObject Email { get; }
    public PhoneValueObject Phone { get; }
    public DocumentValueObject Document { get; }

    private CreateTenantServiceInput(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone, DocumentValueObject document)
    {
        FantasyName = fantasyName;
        LegalName = legalName;
        Email = email;
        Phone = phone;
        Document = document;
    }

    public static CreateTenantServiceInput Factory(FantasyNameValueObject fantasyName, LegalNameValueObject legalName, EmailValueObject email, PhoneValueObject phone,
        DocumentValueObject document)
        => new(fantasyName, legalName, email, phone, document);

    public MethodResult<INotification> GetInputValidation()
        => MethodResult<INotification>.Factory(FantasyName, LegalName, Email, Phone, Document);
}
