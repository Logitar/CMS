using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateFieldTypeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldTypes",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    UniqueName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DataType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Properties = table.Column<string>(type: "text", nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.FieldTypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_AggregateId",
                table: "FieldTypes",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedBy",
                table: "FieldTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedOn",
                table: "FieldTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DataType",
                table: "FieldTypes",
                column: "DataType");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DisplayName",
                table: "FieldTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueId",
                table: "FieldTypes",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueName",
                table: "FieldTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueNameNormalized",
                table: "FieldTypes",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedBy",
                table: "FieldTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedOn",
                table: "FieldTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_Version",
                table: "FieldTypes",
                column: "Version");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldTypes");
        }
    }
}
