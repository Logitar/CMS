using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class CreateContentTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentItems",
                columns: table => new
                {
                    ContentItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItems", x => x.ContentItemId);
                    table.ForeignKey(
                        name: "FK_ContentItems_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentLocales",
                columns: table => new
                {
                    ContentLocaleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UniqueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentTypeId = table.Column<int>(type: "int", nullable: false),
                    ContentItemId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: true),
                    UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentLocales", x => x.ContentLocaleId);
                    table.ForeignKey(
                        name: "FK_ContentLocales_ContentItems_ContentItemId",
                        column: x => x.ContentItemId,
                        principalTable: "ContentItems",
                        principalColumn: "ContentItemId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContentLocales_ContentTypes_ContentTypeId",
                        column: x => x.ContentTypeId,
                        principalTable: "ContentTypes",
                        principalColumn: "ContentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentLocales_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "LanguageId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_AggregateId",
                table: "ContentItems",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_ContentTypeId",
                table: "ContentItems",
                column: "ContentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_CreatedBy",
                table: "ContentItems",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_CreatedOn",
                table: "ContentItems",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_UniqueId",
                table: "ContentItems",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_UpdatedBy",
                table: "ContentItems",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_UpdatedOn",
                table: "ContentItems",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_Version",
                table: "ContentItems",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentItemId",
                table: "ContentLocales",
                column: "ContentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_ContentTypeId_LanguageId_UniqueNameNormalized",
                table: "ContentLocales",
                columns: new[] { "ContentTypeId", "LanguageId", "UniqueNameNormalized" },
                unique: true,
                filter: "[LanguageId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedBy",
                table: "ContentLocales",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_CreatedOn",
                table: "ContentLocales",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_LanguageId",
                table: "ContentLocales",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UniqueId",
                table: "ContentLocales",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UniqueName",
                table: "ContentLocales",
                column: "UniqueName");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedBy",
                table: "ContentLocales",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ContentLocales_UpdatedOn",
                table: "ContentLocales",
                column: "UpdatedOn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentLocales");

            migrationBuilder.DropTable(
                name: "ContentItems");
        }
    }
}
