namespace Ntickets.Application.Services.External.Discord.Inputs;

public readonly struct SignalCreateTenantEventInfoOnChannelDiscordServiceInput
{
    public string TenantId { get; }
    public string FantasyName { get; }
    public string LegalName { get; }
    public string Phone { get; }
    public string Email { get; }

    private SignalCreateTenantEventInfoOnChannelDiscordServiceInput(string tenantId, string fantasyName, string legalName, string phone, string email)
    {
        TenantId = tenantId;
        FantasyName = fantasyName;
        LegalName = legalName;
        Phone = phone;
        Email = email;
    }

    public static SignalCreateTenantEventInfoOnChannelDiscordServiceInput Factory(string tenantId, string fantasyName, string legalName, string phone, string email)
        => new(tenantId, fantasyName, legalName, phone, email);
}
