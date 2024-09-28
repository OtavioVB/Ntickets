﻿using Ntickets.Domain.ValueObjects;

namespace Ntickets.Application.UseCases.CreateTenant.Outputs;

public readonly struct CreateTenantUseCaseOutput
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

    private CreateTenantUseCaseOutput(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status,
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

    public static CreateTenantUseCaseOutput Factory(IdValueObject tenantId, DateTimeValueObject createdAt, TenantStatusValueObject status,
        FantasyNameValueObject fantasyName, LegalNameValueObject legalName, DocumentValueObject document, EmailValueObject email,
        PhoneValueObject phone, DateTimeValueObject lastModifiedAt)
        => new(tenantId, createdAt, status, fantasyName, legalName, document, email, phone, lastModifiedAt);
}