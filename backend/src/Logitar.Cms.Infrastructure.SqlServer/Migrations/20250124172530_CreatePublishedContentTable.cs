using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.Infrastructure.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreatePublishedContentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PublishedContents",
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
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PublishedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PublishedContents", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentLocales_ContentLocaleId",
                        column: x => x.ContentLocaleId,
                        principalTable: "ContentLocales",
                        principalColumn: "ContentLocaleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PublishedContents_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "ContentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PublishedContents_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentId",
                table: "PublishedContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeId",
                table: "PublishedContents",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeName",
                table: "PublishedContents",
                column: "ContentTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentTypeUid",
                table: "PublishedContents",
                column: "ContentTypeUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_ContentUid",
                table: "PublishedContents",
                column: "ContentUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_DisplayName",
                table: "PublishedContents",
                column: "DisplayName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageCode",
                table: "PublishedContents",
                column: "LanguageCode");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageId",
                table: "PublishedContents",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_LanguageUid",
                table: "PublishedContents",
                column: "LanguageUid");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedBy",
                table: "PublishedContents",
                column: "PublishedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_PublishedOn",
                table: "PublishedContents",
                column: "PublishedOn");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueName",
                table: "PublishedContents",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_PublishedContents_UniqueNameNormalized",
                table: "PublishedContents",
                column: "UniqueNameNormalized");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PublishedContents");
        }
    }
}
