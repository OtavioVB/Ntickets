namespace Ntickets.FunctionalTests.UseCases.CreateTenant.Models;

internal class CreateTenantPayloadInput
{
    public CreateTenantPayloadInput(string fantasyName, string legalName, string email, string document, string phone)
    {
        FantasyName = fantasyName;
        LegalName = legalName;
        Email = email;
        Document = document;
        Phone = phone;
    }

    public string FantasyName { get; set; }
    public string LegalName { get; set; }
    public string Email { get; set; }
    public string Document { get; set; }
    public string Phone { get; set; }
}
