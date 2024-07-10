using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateLanguageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    LCID = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    CodeNormalized = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    NativeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_AggregateId",
                table: "Languages",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                table: "Languages",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CodeNormalized",
                table: "Languages",
                column: "CodeNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedBy",
                table: "Languages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_DisplayName",
                table: "Languages",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_EnglishName",
                table: "Languages",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LCID",
                table: "Languages",
                column: "LCID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                table: "Languages",
                column: "NativeName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UniqueId",
                table: "Languages",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedBy",
                table: "Languages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Version",
                table: "Languages",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}
