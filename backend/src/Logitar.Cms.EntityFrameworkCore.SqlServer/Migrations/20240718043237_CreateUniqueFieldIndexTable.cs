using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateUniqueFieldIndexTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StringFieldIndex_FieldDefinitionId",
                table: "StringFieldIndex");

            migrationBuilder.CreateTable(
                name: "UniqueFieldIndex",
                columns: table => new
                {
                    UniqueFieldIndexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentItemId = table.Column<int>(type: "int", nullable: false),
                    ContentItemUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentLocaleUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueFieldIndex", x => x.UniqueFieldIndexId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "StringFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentItemId",
                table: "UniqueFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentItemUid",
                table: "UniqueFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentLocaleId",
                table: "UniqueFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentLocaleName",
                table: "UniqueFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentLocaleUid",
                table: "UniqueFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentTypeId",
                table: "UniqueFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentTypeName",
                table: "UniqueFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_ContentTypeUid",
                table: "UniqueFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "UniqueFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldDefinitionId_LanguageId_Value",
                table: "UniqueFieldIndex",
                columns: new[] { "FieldDefinitionId", "LanguageId", "Value" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldDefinitionName",
                table: "UniqueFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldDefinitionUid",
                table: "UniqueFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldTypeId",
                table: "UniqueFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldTypeName",
                table: "UniqueFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_FieldTypeUid",
                table: "UniqueFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_LanguageCode",
                table: "UniqueFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_LanguageId",
                table: "UniqueFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_LanguageUid",
                table: "UniqueFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueFieldIndex_Value",
                table: "UniqueFieldIndex",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniqueFieldIndex");

            migrationBuilder.DropIndex(
                name: "IX_StringFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "StringFieldIndex");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldDefinitionId",
                table: "StringFieldIndex",
                column: "FieldDefinitionId");
        }
    }
}
