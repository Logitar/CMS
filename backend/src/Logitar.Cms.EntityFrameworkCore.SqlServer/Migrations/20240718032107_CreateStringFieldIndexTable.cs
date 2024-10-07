using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateStringFieldIndexTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StringFieldIndex",
                columns: table => new
                {
                    StringFieldIndexId = table.Column<int>(type: "int", nullable: false)
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
                    table.PrimaryKey("PK_StringFieldIndex", x => x.StringFieldIndexId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentItemId",
                table: "StringFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentItemUid",
                table: "StringFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentLocaleId",
                table: "StringFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentLocaleName",
                table: "StringFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentLocaleUid",
                table: "StringFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentTypeId",
                table: "StringFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentTypeName",
                table: "StringFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_ContentTypeUid",
                table: "StringFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldDefinitionId",
                table: "StringFieldIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldDefinitionName",
                table: "StringFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldDefinitionUid",
                table: "StringFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldTypeId",
                table: "StringFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldTypeName",
                table: "StringFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_FieldTypeUid",
                table: "StringFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_LanguageCode",
                table: "StringFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_LanguageId",
                table: "StringFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_LanguageUid",
                table: "StringFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_StringFieldIndex_Value",
                table: "StringFieldIndex",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StringFieldIndex");
        }
    }
}
