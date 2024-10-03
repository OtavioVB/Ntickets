using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ntickets.Infrascructure.Migrations
{
    /// <inheritdoc />
    public partial class add_table_tenants_on_database : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenant");

            migrationBuilder.CreateTable(
                name: "tenants",
                schema: "tenant",
                columns: table => new
                {
                    idtenant = table.Column<string>(type: "VARCHAR", fixedLength: true, maxLength: 26, nullable: false),
                    created_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false),
                    fantasy_name = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: false),
                    legal_name = table.Column<string>(type: "VARCHAR", maxLength: 64, nullable: false),
                    document = table.Column<string>(type: "VARCHAR", maxLength: 14, nullable: false),
                    email = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    phone = table.Column<string>(type: "CHAR(13)", fixedLength: true, maxLength: 13, nullable: false),
                    status = table.Column<string>(type: "VARCHAR", maxLength: 255, nullable: false),
                    last_modified_at = table.Column<DateTime>(type: "TIMESTAMPTZ", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_id", x => x.idtenant);
                });

            migrationBuilder.CreateIndex(
                name: "ik_asc_tenant_id",
                schema: "tenant",
                table: "tenants",
                column: "idtenant");

            migrationBuilder.CreateIndex(
                name: "uk_tenant_document_number",
                schema: "tenant",
                table: "tenants",
                column: "document",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tenants",
                schema: "tenant");
        }
    }
}
