using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Cms_Release_1_0_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cms");

            migrationBuilder.CreateTable(
                name: "ContentTypes",
                schema: "Cms",
                columns: table => new
                {
                    ContentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsInvariant = table.Column<bool>(type: "bit", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldCount = table.Column<int>(type: "int", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTypes", x => x.ContentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "FieldTypes",
                schema: "Cms",
                columns: table => new
                {
                    FieldTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldTypes", x => x.FieldTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "Cms",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    LCID = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CodeNormalized = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EnglishName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                schema: "Cms",
                columns: table => new
                {
                    ContentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    StreamId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => x.ContentId);
                    table.ForeignKey(
                        name: "FK_Contents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldDefinitions",
                schema: "Cms",
                columns: table => new
                {
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Placeholder = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldDefinitions", x => x.FieldDefinitionId);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldDefinitions_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Cms",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentLocales",
                schema: "Cms",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    PublishedRevision = table.Column<long>(type: "bigint", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLocales", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_ContentLocales_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Cms",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Cms",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FieldIndex",
                schema: "Cms",
                columns: table => new
                {
                    FieldIndexId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentLocaleName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Boolean = table.Column<bool>(type: "bit", nullable: true),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Number = table.Column<double>(type: "float", nullable: true),
                    RelatedContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RichText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Select = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    String = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldIndex", x => x.FieldIndexId);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Cms",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Cms",
                        principalTable: "Contents",
                        principalColumn: "ContentId");
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Cms",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId");
                    table.ForeignKey(
                        name: "FK_FieldIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Cms",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Cms",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PublishedContents",
                schema: "Cms",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    ContentUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    LanguageUid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: true),
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
                    PublishedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedContents", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalSchema: "Cms",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Cms",
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Cms",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UniqueIndex",
                schema: "Cms",
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
                    LanguageIsDefault = table.Column<bool>(type: "bit", nullable: false),
                    FieldTypeId = table.Column<int>(type: "int", nullable: false),
                    FieldTypeUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldTypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FieldDefinitionId = table.Column<int>(type: "int", nullable: false),
                    FieldDefinitionUid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FieldDefinitionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Revision = table.Column<long>(type: "bigint", nullable: false),
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
                        principalSchema: "Cms",
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalSchema: "Cms",
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Contents_ContentId",
                        column: x => x.ContentId,
                        principalSchema: "Cms",
                        principalTable: "Contents",
                        principalColumn: "ContentId");
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldDefinitions_FieldDefinitionId",
                        column: x => x.FieldDefinitionId,
                        principalSchema: "Cms",
                        principalTable: "FieldDefinitions",
                        principalColumn: "FieldDefinitionId");
                    table.ForeignKey(
                        name: "FK_UniqueIndex_FieldTypes_FieldTypeId",
                        column: x => x.FieldTypeId,
                        principalSchema: "Cms",
                        principalTable: "FieldTypes",
                        principalColumn: "FieldTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueIndex_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Cms",
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentId_LanguageId",
                schema: "Cms",
                table: "ContentLocales",
                columns: new[] { "ContentId", "LanguageId" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeId_LanguageId_UniqueNameNormalized",
                schema: "Cms",
                table: "ContentLocales",
                columns: new[] { "ContentTypeId", "LanguageId", "UniqueNameNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedBy",
                schema: "Cms",
                table: "ContentLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedOn",
                schema: "Cms",
                table: "ContentLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_DisplayName",
                schema: "Cms",
                table: "ContentLocales",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_IsPublished",
                schema: "Cms",
                table: "ContentLocales",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageId",
                schema: "Cms",
                table: "ContentLocales",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedBy",
                schema: "Cms",
                table: "ContentLocales",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedOn",
                schema: "Cms",
                table: "ContentLocales",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_PublishedRevision",
                schema: "Cms",
                table: "ContentLocales",
                column: "PublishedRevision");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_Revision",
                schema: "Cms",
                table: "ContentLocales",
                column: "Revision");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UniqueName",
                schema: "Cms",
                table: "ContentLocales",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedBy",
                schema: "Cms",
                table: "ContentLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedOn",
                schema: "Cms",
                table: "ContentLocales",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_ContentTypeId",
                schema: "Cms",
                table: "Contents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedBy",
                schema: "Cms",
                table: "Contents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_CreatedOn",
                schema: "Cms",
                table: "Contents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Id",
                schema: "Cms",
                table: "Contents",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_StreamId",
                schema: "Cms",
                table: "Contents",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedBy",
                schema: "Cms",
                table: "Contents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UpdatedOn",
                schema: "Cms",
                table: "Contents",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Contents_Version",
                schema: "Cms",
                table: "Contents",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedBy",
                schema: "Cms",
                table: "ContentTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_CreatedOn",
                schema: "Cms",
                table: "ContentTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_DisplayName",
                schema: "Cms",
                table: "ContentTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_FieldCount",
                schema: "Cms",
                table: "ContentTypes",
                column: "FieldCount");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_Id",
                schema: "Cms",
                table: "ContentTypes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_IsInvariant",
                schema: "Cms",
                table: "ContentTypes",
                column: "IsInvariant");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_StreamId",
                schema: "Cms",
                table: "ContentTypes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueName",
                schema: "Cms",
                table: "ContentTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UniqueNameNormalized",
                schema: "Cms",
                table: "ContentTypes",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedBy",
                schema: "Cms",
                table: "ContentTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_UpdatedOn",
                schema: "Cms",
                table: "ContentTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentTypes_Version",
                schema: "Cms",
                table: "ContentTypes",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_Id",
                schema: "Cms",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "Id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_Order",
                schema: "Cms",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_ContentTypeId_UniqueNameNormalized",
                schema: "Cms",
                table: "FieldDefinitions",
                columns: new[] { "ContentTypeId", "UniqueNameNormalized" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldDefinitions_FieldTypeId",
                schema: "Cms",
                table: "FieldDefinitions",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Boolean",
                schema: "Cms",
                table: "FieldIndex",
                column: "Boolean");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentId",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleId_FieldDefinitionId_Status",
                schema: "Cms",
                table: "FieldIndex",
                columns: new[] { "ContentLocaleId", "FieldDefinitionId", "Status" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentLocaleName",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeId",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeName",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentTypeUid",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_ContentUid",
                schema: "Cms",
                table: "FieldIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_DateTime",
                schema: "Cms",
                table: "FieldIndex",
                column: "DateTime");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionId",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionName",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldDefinitionUid",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeId",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeName",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_FieldTypeUid",
                schema: "Cms",
                table: "FieldIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageCode",
                schema: "Cms",
                table: "FieldIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageId",
                schema: "Cms",
                table: "FieldIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageIsDefault",
                schema: "Cms",
                table: "FieldIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_LanguageUid",
                schema: "Cms",
                table: "FieldIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Number",
                schema: "Cms",
                table: "FieldIndex",
                column: "Number");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Revision",
                schema: "Cms",
                table: "FieldIndex",
                column: "Revision");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_Status",
                schema: "Cms",
                table: "FieldIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FieldIndex_String",
                schema: "Cms",
                table: "FieldIndex",
                column: "String");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedBy",
                schema: "Cms",
                table: "FieldTypes",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_CreatedOn",
                schema: "Cms",
                table: "FieldTypes",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DataType",
                schema: "Cms",
                table: "FieldTypes",
                column: "DataType");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_DisplayName",
                schema: "Cms",
                table: "FieldTypes",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_Id",
                schema: "Cms",
                table: "FieldTypes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_StreamId",
                schema: "Cms",
                table: "FieldTypes",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueName",
                schema: "Cms",
                table: "FieldTypes",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UniqueNameNormalized",
                schema: "Cms",
                table: "FieldTypes",
                column: "UniqueNameNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedBy",
                schema: "Cms",
                table: "FieldTypes",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_UpdatedOn",
                schema: "Cms",
                table: "FieldTypes",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_FieldTypes_Version",
                schema: "Cms",
                table: "FieldTypes",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Code",
                schema: "Cms",
                table: "Languages",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CodeNormalized",
                schema: "Cms",
                table: "Languages",
                column: "CodeNormalized",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedBy",
                schema: "Cms",
                table: "Languages",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                schema: "Cms",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_DisplayName",
                schema: "Cms",
                table: "Languages",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_EnglishName",
                schema: "Cms",
                table: "Languages",
                column: "EnglishName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Id",
                schema: "Cms",
                table: "Languages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                schema: "Cms",
                table: "Languages",
                column: "IsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LCID",
                schema: "Cms",
                table: "Languages",
                column: "LCID");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_NativeName",
                schema: "Cms",
                table: "Languages",
                column: "NativeName");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_StreamId",
                schema: "Cms",
                table: "Languages",
                column: "StreamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedBy",
                schema: "Cms",
                table: "Languages",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "Cms",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Version",
                schema: "Cms",
                table: "Languages",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentId",
                schema: "Cms",
                table: "PublishedContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeId",
                schema: "Cms",
                table: "PublishedContents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeName",
                schema: "Cms",
                table: "PublishedContents",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeUid",
                schema: "Cms",
                table: "PublishedContents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentUid",
                schema: "Cms",
                table: "PublishedContents",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_DisplayName",
                schema: "Cms",
                table: "PublishedContents",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageCode",
                schema: "Cms",
                table: "PublishedContents",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageId",
                schema: "Cms",
                table: "PublishedContents",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageIsDefault",
                schema: "Cms",
                table: "PublishedContents",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageUid",
                schema: "Cms",
                table: "PublishedContents",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedBy",
                schema: "Cms",
                table: "PublishedContents",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedOn",
                schema: "Cms",
                table: "PublishedContents",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_Revision",
                schema: "Cms",
                table: "PublishedContents",
                column: "Revision");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueName",
                schema: "Cms",
                table: "PublishedContents",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueNameNormalized",
                schema: "Cms",
                table: "PublishedContents",
                column: "UniqueNameNormalized");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentLocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentLocaleName",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentLocaleName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeName",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentTypeUid",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ContentUid",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionId_LanguageId_Status_ValueNormalized",
                schema: "Cms",
                table: "UniqueIndex",
                columns: new[] { "FieldDefinitionId", "LanguageId", "Status", "ValueNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionName",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldDefinitionName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldDefinitionUid",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldDefinitionUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeName",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_FieldTypeUid",
                schema: "Cms",
                table: "UniqueIndex",
                column: "FieldTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Key",
                schema: "Cms",
                table: "UniqueIndex",
                column: "Key");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageCode",
                schema: "Cms",
                table: "UniqueIndex",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageId",
                schema: "Cms",
                table: "UniqueIndex",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageIsDefault",
                schema: "Cms",
                table: "UniqueIndex",
                column: "LanguageIsDefault");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_LanguageUid",
                schema: "Cms",
                table: "UniqueIndex",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Revision",
                schema: "Cms",
                table: "UniqueIndex",
                column: "Revision");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Status",
                schema: "Cms",
                table: "UniqueIndex",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_Value",
                schema: "Cms",
                table: "UniqueIndex",
                column: "Value");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueIndex_ValueNormalized",
                schema: "Cms",
                table: "UniqueIndex",
                column: "ValueNormalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldIndex",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "PublishedContents",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "UniqueIndex",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "ContentLocales",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "FieldDefinitions",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "Contents",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "FieldTypes",
                schema: "Cms");

            migrationBuilder.DropTable(
                name: "ContentTypes",
                schema: "Cms");
        }
    }
}
