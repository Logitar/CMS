using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
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
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Locale = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    LocaleNormalized = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_Languages_Id",
                table: "Languages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Locale",
                table: "Languages",
                column: "Locale");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LocaleNormalized",
                table: "Languages",
                column: "LocaleNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                table: "Languages",
                column: "NativeName");

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
