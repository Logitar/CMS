using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLanguageColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocaleNormalized",
                table: "Languages",
                newName: "CodeNormalized");

            migrationBuilder.RenameColumn(
                name: "Locale",
                table: "Languages",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_LocaleNormalized",
                table: "Languages",
                newName: "IX_Languages_CodeNormalized");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_Locale",
                table: "Languages",
                newName: "IX_Languages_Code");

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Languages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "Languages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LCID",
                table: "Languages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "NativeName",
                table: "Languages",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_DisplayName",
                table: "Languages",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_EnglishName",
                table: "Languages",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LCID",
                table: "Languages",
                column: "LCID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                table: "Languages",
                column: "NativeName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Languages_DisplayName",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_EnglishName",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LCID",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_NativeName",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "LCID",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "NativeName",
                table: "Languages");

            migrationBuilder.RenameColumn(
                name: "CodeNormalized",
                table: "Languages",
                newName: "LocaleNormalized");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Languages",
                newName: "Locale");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_CodeNormalized",
                table: "Languages",
                newName: "IX_Languages_LocaleNormalized");

            migrationBuilder.RenameIndex(
                name: "IX_Languages_Code",
                table: "Languages",
                newName: "IX_Languages_Locale");
        }
    }
}
