using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject;
using Ntickets.Domain.BoundedContexts.TenantContext.Enumerators;
using Ntickets.Domain.ValueObjects;

namespace Ntickets.Infrascructure.EntityFrameworkCore.Mappings;

public sealed class TenantMapping : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        #region Table Configuration

        const string SCHEMA_NAME = "tenant";
        const string TABLE_NAME = "tenants";

        builder.ToTable(
            name: TABLE_NAME,
            schema: SCHEMA_NAME);

        #endregion

        #region Primary Key Configuration

        const string PRIMARY_KEY_CONSTRAINT_NAME = "pk_tenant_id";

        builder.HasKey(p => p.TenantId)
            .HasName(PRIMARY_KEY_CONSTRAINT_NAME);

        #endregion

        #region Foreign Key Configuration

        #endregion

        #region Index Key Configuration

        const string DOCUMENT_UNIQUE_KEY_CONSTRAINT_NAME = "uk_tenant_document_number";

        builder.HasIndex(p => p.Document)
            .HasDatabaseName(DOCUMENT_UNIQUE_KEY_CONSTRAINT_NAME)
            .IsUnique(true);

        const string TENANT_ID_ASCENDING_INDEX_KEY_CONSTRAINT_NAME = "ik_asc_tenant_id";

        builder.HasIndex(p => p.TenantId)
            .HasDatabaseName(TENANT_ID_ASCENDING_INDEX_KEY_CONSTRAINT_NAME)
            .IsDescending(false);

        #endregion

        #region Properties Configuration

        builder.Property(p => p.TenantId)
            .IsRequired(true)
            .IsFixedLength(true)
            .HasColumnType("VARCHAR")
            .HasColumnName("idtenant")
            .HasMaxLength(Ulid.Empty.ToString().Length)
            .ValueGeneratedNever();
        builder.Property(p => p.CreatedAt)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnType("TIMESTAMPTZ")
            .HasColumnName("created_at")
            .ValueGeneratedNever();
        builder.Property(p => p.FantasyName)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnName("fantasy_name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(FantasyNameValueObject.MAX_LENGTH)
            .ValueGeneratedNever();
        builder.Property(p => p.LegalName)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnName("legal_name")
            .HasColumnType("VARCHAR")
            .HasMaxLength(LegalNameValueObject.MAX_LENGTH)
            .ValueGeneratedNever();
        builder.Property(p => p.Document)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnName("document")
            .HasColumnType("VARCHAR")
            .HasMaxLength(DocumentValueObject.CNPJ_LENGTH)
            .ValueGeneratedNever();
        builder.Property(p => p.Email)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnName("email")
            .HasColumnType("VARCHAR")
            .HasMaxLength(EmailValueObject.MAX_LENGTH)
            .ValueGeneratedNever();
        builder.Property(p => p.Phone)
            .IsRequired(true)
            .IsFixedLength(true)
            .HasColumnName("phone")
            .HasColumnType("CHAR")
            .HasMaxLength(PhoneValueObject.EXPECTED_LENGTH)
            .ValueGeneratedNever();
        builder.Property(p => p.Status)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnName("status")
            .HasColumnType("VARCHAR")
            .HasMaxLength(255)
            .HasConversion(p => p.ToString(), p => Enum.Parse<EnumTenantStatus>(p))
            .ValueGeneratedNever();
        builder.Property(p => p.LastModifiedAt)
            .IsRequired(true)
            .IsFixedLength(false)
            .HasColumnType("TIMESTAMPTZ")
            .HasColumnName("last_modified_at")
            .ValueGeneratedNever();

        #endregion
    }
}
