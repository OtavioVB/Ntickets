﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Ntickets.Infrascructure.EntityFrameworkCore;

#nullable disable

namespace Ntickets.Infrascructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20241003141957_add_table_tenants_on_database")]
    partial class add_table_tenants_on_database
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Ntickets.Domain.BoundedContexts.TenantContext.DataTransferObject.Tenant", b =>
                {
                    b.Property<string>("TenantId")
                        .HasMaxLength(26)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("idtenant")
                        .IsFixedLength();

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TIMESTAMPTZ")
                        .HasColumnName("created_at")
                        .IsFixedLength(false);

                    b.Property<string>("Document")
                        .IsRequired()
                        .HasMaxLength(14)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("document")
                        .IsFixedLength(false);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("email")
                        .IsFixedLength(false);

                    b.Property<string>("FantasyName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("fantasy_name")
                        .IsFixedLength(false);

                    b.Property<DateTime>("LastModifiedAt")
                        .HasColumnType("TIMESTAMPTZ")
                        .HasColumnName("last_modified_at")
                        .IsFixedLength(false);

                    b.Property<string>("LegalName")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("legal_name")
                        .IsFixedLength(false);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("CHAR")
                        .HasColumnName("phone")
                        .IsFixedLength();

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR")
                        .HasColumnName("status")
                        .IsFixedLength(false);

                    b.HasKey("TenantId")
                        .HasName("pk_tenant_id");

                    b.HasIndex("Document")
                        .IsUnique()
                        .HasDatabaseName("uk_tenant_document_number");

                    b.HasIndex("TenantId")
                        .HasDatabaseName("ik_asc_tenant_id");

                    b.ToTable("tenants", "tenant");
                });
#pragma warning restore 612, 618
        }
    }
}
