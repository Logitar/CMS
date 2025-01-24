using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddContentLocalePublicationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "ContentLocales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublishedBy",
                table: "ContentLocales",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedOn",
                table: "ContentLocales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_IsPublished",
                table: "ContentLocales",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedBy",
                table: "ContentLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedOn",
                table: "ContentLocales",
                column: "PublishedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_IsPublished",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_PublishedBy",
                table: "ContentLocales");

            migrationBuilder.DropIndex(
                name: "IX_ContentLocales_PublishedOn",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "PublishedBy",
                table: "ContentLocales");

            migrationBuilder.DropColumn(
                name: "PublishedOn",
                table: "ContentLocales");
        }
    }
}
