using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Logitar.Cms.EntityFrameworkCore.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class CreateLogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UniqueId = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Method = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Destination = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    Source = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    OperationType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    OperationName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActivityData = table.Column<string>(type: "text", nullable: true),
                    StatusCode = table.Column<int>(type: "integer", nullable: true),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Level = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    HasErrors = table.Column<bool>(type: "boolean", nullable: false),
                    StartedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TenantId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ActorId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ApiKeyId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    SessionId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "LogEvents",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    LogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogEvents", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_LogEvents_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogExceptions",
                columns: table => new
                {
                    LogExceptionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogId = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    HResult = table.Column<int>(type: "integer", nullable: false),
                    HelpLink = table.Column<string>(type: "text", nullable: true),
                    Source = table.Column<string>(type: "text", nullable: true),
                    StackTrace = table.Column<string>(type: "text", nullable: true),
                    TargetSite = table.Column<string>(type: "text", nullable: true),
                    Data = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogExceptions", x => x.LogExceptionId);
                    table.ForeignKey(
                        name: "FK_LogExceptions_Logs_LogId",
                        column: x => x.LogId,
                        principalTable: "Logs",
                        principalColumn: "LogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogEvents_LogId",
                table: "LogEvents",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_LogId",
                table: "LogExceptions",
                column: "LogId");

            migrationBuilder.CreateIndex(
                name: "IX_LogExceptions_Type",
                table: "LogExceptions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActivityType",
                table: "Logs",
                column: "ActivityType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ActorId",
                table: "Logs",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ApiKeyId",
                table: "Logs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_CorrelationId",
                table: "Logs",
                column: "CorrelationId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Duration",
                table: "Logs",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_EndedOn",
                table: "Logs",
                column: "EndedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_HasErrors",
                table: "Logs",
                column: "HasErrors");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IsCompleted",
                table: "Logs",
                column: "IsCompleted");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationName",
                table: "Logs",
                column: "OperationName");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_OperationType",
                table: "Logs",
                column: "OperationType");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SessionId",
                table: "Logs",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StartedOn",
                table: "Logs",
                column: "StartedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_StatusCode",
                table: "Logs",
                column: "StatusCode");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_TenantId",
                table: "Logs",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UniqueId",
                table: "Logs",
                column: "UniqueId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_UserId",
                table: "Logs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogEvents");

            migrationBuilder.DropTable(
                name: "LogExceptions");

            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
