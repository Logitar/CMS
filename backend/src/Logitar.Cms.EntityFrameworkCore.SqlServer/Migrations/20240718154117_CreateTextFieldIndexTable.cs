using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateTextFieldIndexTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TextFieldIndex",
                columns: table => new
                {
                    TextFieldIndexId = table.Column<int>(type: "int", nullable: false)
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
                    Value = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextFieldIndex", x => x.TextFieldIndexId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentItemId",
                table: "TextFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentItemUid",
                table: "TextFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentLocaleId",
                table: "TextFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentLocaleName",
                table: "TextFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentLocaleUid",
                table: "TextFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentTypeId",
                table: "TextFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentTypeName",
                table: "TextFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_ContentTypeUid",
                table: "TextFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "TextFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldDefinitionName",
                table: "TextFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldDefinitionUid",
                table: "TextFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldTypeId",
                table: "TextFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldTypeName",
                table: "TextFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_FieldTypeUid",
                table: "TextFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_LanguageCode",
                table: "TextFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_LanguageId",
                table: "TextFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_LanguageUid",
                table: "TextFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_TextFieldIndex_Value",
                table: "TextFieldIndex",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TextFieldIndex");
        }
    }
}
