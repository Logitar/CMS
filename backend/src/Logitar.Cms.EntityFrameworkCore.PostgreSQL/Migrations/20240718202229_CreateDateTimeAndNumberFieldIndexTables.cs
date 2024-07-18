using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateDateTimeAndNumberFieldIndexTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DateTimeFieldIndex",
                columns: table => new
                {
                    DateTimeFieldIndexId = table.Column<int>(type: "integer", nullable: false)
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
                    Value = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTimeFieldIndex", x => x.DateTimeFieldIndexId);
                });

            migrationBuilder.CreateTable(
                name: "NumberFieldIndex",
                columns: table => new
                {
                    NumberFieldIndexId = table.Column<int>(type: "integer", nullable: false)
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
                    Value = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberFieldIndex", x => x.NumberFieldIndexId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentItemId",
                table: "DateTimeFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentItemUid",
                table: "DateTimeFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentLocaleId",
                table: "DateTimeFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentLocaleName",
                table: "DateTimeFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentLocaleUid",
                table: "DateTimeFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentTypeId",
                table: "DateTimeFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentTypeName",
                table: "DateTimeFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_ContentTypeUid",
                table: "DateTimeFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "DateTimeFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldDefinitionName",
                table: "DateTimeFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldDefinitionUid",
                table: "DateTimeFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldTypeId",
                table: "DateTimeFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldTypeName",
                table: "DateTimeFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_FieldTypeUid",
                table: "DateTimeFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_LanguageCode",
                table: "DateTimeFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_LanguageId",
                table: "DateTimeFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_LanguageUid",
                table: "DateTimeFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_DateTimeFieldIndex_Value",
                table: "DateTimeFieldIndex",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentItemId",
                table: "NumberFieldIndex",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentItemUid",
                table: "NumberFieldIndex",
                column: "ContentItemUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentLocaleId",
                table: "NumberFieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentLocaleName",
                table: "NumberFieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentLocaleUid",
                table: "NumberFieldIndex",
                column: "ContentLocaleUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentTypeId",
                table: "NumberFieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentTypeName",
                table: "NumberFieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_ContentTypeUid",
                table: "NumberFieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldDefinitionId_ContentLocaleId",
                table: "NumberFieldIndex",
                columns: new[] { "FieldDefinitionId", "ContentLocaleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldDefinitionName",
                table: "NumberFieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldDefinitionUid",
                table: "NumberFieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldTypeId",
                table: "NumberFieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldTypeName",
                table: "NumberFieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_FieldTypeUid",
                table: "NumberFieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_LanguageCode",
                table: "NumberFieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_LanguageId",
                table: "NumberFieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_LanguageUid",
                table: "NumberFieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_NumberFieldIndex_Value",
                table: "NumberFieldIndex",
                column: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DateTimeFieldIndex");

            migrationBuilder.DropTable(
                name: "NumberFieldIndex");
        }
    }
}
