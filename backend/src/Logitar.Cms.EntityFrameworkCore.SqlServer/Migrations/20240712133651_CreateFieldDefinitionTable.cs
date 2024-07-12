using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateFieldDefinitionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                columns: table => new
                {
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    IsInvariant = table.Column<bool>(type: "bit", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    IsIndexed = table.Column<bool>(type: "bit", nullable: false),
                    IsUnique = table.Column<bool>(type: "bit", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Placeholder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinitions", x => x.FieldDefinitionId);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_Order",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_UniqueNameNormalized",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_CreatedBy",
                table: "FieldDefinitions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_CreatedOn",
                table: "FieldDefinitions",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_DisplayName",
                table: "FieldDefinitions",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_FieldTypeId",
                table: "FieldDefinitions",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsIndexed",
                table: "FieldDefinitions",
                column: "IsIndexed");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsInvariant",
                table: "FieldDefinitions",
                column: "IsInvariant");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsRequired",
                table: "FieldDefinitions",
                column: "IsRequired");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_IsUnique",
                table: "FieldDefinitions",
                column: "IsUnique");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UniqueId",
                table: "FieldDefinitions",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UniqueName",
                table: "FieldDefinitions",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UpdatedBy",
                table: "FieldDefinitions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_UpdatedOn",
                table: "FieldDefinitions",
                column: "UpdatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldDefinitions");
        }
    }
}
