using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateContentTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentTypes",
                columns: table => new
                {
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsInvariant = table.Column<bool>(type: "boolean", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.ContentTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_AggregateId",
                table: "ContentTypes",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedBy",
                table: "ContentTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedOn",
                table: "ContentTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_DisplayName",
                table: "ContentTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_IsInvariant",
                table: "ContentTypes",
                column: "IsInvariant");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueId",
                table: "ContentTypes",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueName",
                table: "ContentTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueNameNormalized",
                table: "ContentTypes",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedBy",
                table: "ContentTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedOn",
                table: "ContentTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_Version",
                table: "ContentTypes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentTypes");
        }
    }
}
