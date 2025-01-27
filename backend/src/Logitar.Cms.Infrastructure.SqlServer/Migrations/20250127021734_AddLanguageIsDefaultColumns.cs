using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageIsDefaultColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LanguageIsDefault",
                table: "UniqueIndex",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LanguageIsDefault",
                table: "PublishedContents",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LanguageIsDefault",
                table: "FieldIndex",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageIsDefault",
                table: "UniqueIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageIsDefault",
                table: "PublishedContents",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageIsDefault",
                table: "FieldIndex",
                column: "LanguageIsDefault");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UniqueIndex_LanguageIsDefault",
                table: "UniqueIndex");

            migrationBuilder.DropIndex(
                name: "IX_PublishedContents_LanguageIsDefault",
                table: "PublishedContents");

            migrationBuilder.DropIndex(
                name: "IX_FieldIndex_LanguageIsDefault",
                table: "FieldIndex");

            migrationBuilder.DropColumn(
                name: "LanguageIsDefault",
                table: "UniqueIndex");

            migrationBuilder.DropColumn(
                name: "LanguageIsDefault",
                table: "PublishedContents");

            migrationBuilder.DropColumn(
                name: "LanguageIsDefault",
                table: "FieldIndex");
        }
    }
}
