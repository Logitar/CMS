using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cms");

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "cms",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Locale = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "cms",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    UsernameNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordChangedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    PasswordChangedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Password = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    HasPassword = table.Column<bool>(type: "boolean", nullable: false),
                    DisabledById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    DisabledOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    SignedInOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmailAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailAddressNormalized = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmailVerifiedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    Locale = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Picture = table.Column<string>(type: "character varying(65535)", maxLength: 65535, nullable: true),
                    AggregateId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Languages_AggregateId",
                schema: "cms",
                table: "Languages",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_CreatedOn",
                schema: "cms",
                table: "Languages",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_IsDefault",
                schema: "cms",
                table: "Languages",
                column: "IsDefault",
                unique: true,
                filter: "\"IsDefault\" = true");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_Locale",
                schema: "cms",
                table: "Languages",
                column: "Locale",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Languages_UpdatedOn",
                schema: "cms",
                table: "Languages",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AggregateId",
                schema: "cms",
                table: "Users",
                column: "AggregateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedOn",
                schema: "cms",
                table: "Users",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedOn",
                schema: "cms",
                table: "Users",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UsernameNormalized",
                schema: "cms",
                table: "Users",
                column: "UsernameNormalized",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "cms");
        }
    }
}
