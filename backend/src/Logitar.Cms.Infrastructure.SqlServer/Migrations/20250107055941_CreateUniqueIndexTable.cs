using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateUniqueIndexTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniqueIndex",
                columns: table => new
                {
                    UniqueIndexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ValueNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(278)", maxLength: 278, nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueIndex", x => x.UniqueIndexId);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentId",
                table: "UniqueIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleId",
                table: "UniqueIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleName",
                table: "UniqueIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeId",
                table: "UniqueIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeName",
                table: "UniqueIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeUid",
                table: "UniqueIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentUid",
                table: "UniqueIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId",
                table: "UniqueIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId_LanguageId_ValueNormalized",
                table: "UniqueIndex",
                columns: new[] { "FieldDefinitionId", "LanguageId", "ValueNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionName",
                table: "UniqueIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionUid",
                table: "UniqueIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeId",
                table: "UniqueIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeName",
                table: "UniqueIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeUid",
                table: "UniqueIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Key",
                table: "UniqueIndex",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageCode",
                table: "UniqueIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageId",
                table: "UniqueIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageUid",
                table: "UniqueIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Status",
                table: "UniqueIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Value",
                table: "UniqueIndex",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ValueNormalized",
                table: "UniqueIndex",
                column: "ValueNormalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniqueIndex");
        }
    }
}
