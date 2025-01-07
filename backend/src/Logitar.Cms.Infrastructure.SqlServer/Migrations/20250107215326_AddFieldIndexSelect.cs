using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldIndexSelect : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Select",
                table: "FieldIndex",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Select",
                table: "FieldIndex");
        }
    }
}
