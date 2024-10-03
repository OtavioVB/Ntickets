namespace Ntickets.WebApi.Controllers.TenantContext.Payloads;

public readonly struct CreateTenantPayloadInput
{
    public CreateTenantPayloadInput(string fantasyName, string legalName, string document, string email, string phone)
    {
        FantasyName = fantasyName;
        LegalName = legalName;
        Document = document;
        Email = email;
        Phone = phone;
    }

    public string FantasyName { get; init; }
    public string LegalName { get; init; }
    public string Document { get; init; }
    public string Email { get; init; }
    public string Phone { get; init; }
}
