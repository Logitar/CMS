using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldIndexTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "FieldIndex",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tags",
                table: "FieldIndex");
        }
    }
}
