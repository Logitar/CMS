using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateBooleanFieldIndexTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BooleanFieldIndex",
                columns: table => new
                {
                    BooleanFieldIndexId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContentTypeId = table.Column<int>(type: "integer", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FieldTypeId = table.Column<int>(type: "integer", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldTypeName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "integer", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentItemId = table.Column<int>(type: "integer", nullable: false),
                    ContentItemUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "integer", nullable: false),
                    ContentLocaleUid = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "integer", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uuid", nullable: true),
                    LanguageCode = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    Value = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BooleanFieldIndex", x => x.BooleanFieldIndexId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentItemId",
                table: "BooleanFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentItemUid",
                table: "BooleanFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentLocaleId",
                table: "BooleanFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentLocaleName",
                table: "BooleanFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentLocaleUid",
                table: "BooleanFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentTypeId",
                table: "BooleanFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentTypeName",
                table: "BooleanFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_ContentTypeUid",
                table: "BooleanFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "BooleanFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldDefinitionName",
                table: "BooleanFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldDefinitionUid",
                table: "BooleanFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldTypeId",
                table: "BooleanFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldTypeName",
                table: "BooleanFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_FieldTypeUid",
                table: "BooleanFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_LanguageCode",
                table: "BooleanFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_LanguageId",
                table: "BooleanFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_LanguageUid",
                table: "BooleanFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_BooleanFieldIndex_Value",
                table: "BooleanFieldIndex",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BooleanFieldIndex");
        }
    }
}
