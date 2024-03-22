using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.SqlServer.Migrations
{
  /// <inheritdoc />
  public partial class CreateArchetypeTable : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
          name: "Archetypes",
          columns: table => new
          {
            ArchetypeId = table.Column<int>(type: "int", nullable: false)
                  .Annotation("SqlServer:Identity", "1, 1"),
            IsInvariant = table.Column<bool>(type: "bit", nullable: false),
            UniqueName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            UniqueNameNormalized = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            DisplayName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
            Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
            AggregateId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            Version = table.Column<long>(type: "bigint", nullable: false),
            CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
            UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table =>
          {
            table.PrimaryKey("PK_Archetypes", x => x.ArchetypeId);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_AggregateId",
          table: "Archetypes",
          column: "AggregateId",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_CreatedBy",
          table: "Archetypes",
          column: "CreatedBy");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_CreatedOn",
          table: "Archetypes",
          column: "CreatedOn");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_DisplayName",
          table: "Archetypes",
          column: "DisplayName");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_IsInvariant",
          table: "Archetypes",
          column: "IsInvariant");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_UniqueName",
          table: "Archetypes",
          column: "UniqueName");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_UniqueNameNormalized",
          table: "Archetypes",
          column: "UniqueNameNormalized",
          unique: true);

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_UpdatedBy",
          table: "Archetypes",
          column: "UpdatedBy");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_UpdatedOn",
          table: "Archetypes",
          column: "UpdatedOn");

      migrationBuilder.CreateIndex(
          name: "IX_Archetypes_Version",
          table: "Archetypes",
          column: "Version");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(
          name: "Archetypes");
    }
  }
}
