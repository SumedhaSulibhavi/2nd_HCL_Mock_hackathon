using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HypeHealthAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SprintVelocity = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyMoodLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    LogDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyMoodLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DailyMoodLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KudosCards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ReceiverId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KudosCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KudosCards_Users_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KudosCards_Users_SenderId",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "PasswordHash", "Role", "SprintVelocity", "Username" },
                values: new object[,]
                {
                    { 1, "emp@company.com", "$2a$11$mSeJEUK/usSIy52Lb7NjHu2M91Ll3327m16OM8hQcLG5IuCw.9B5K", "Employee", 42.50m, "Sanjana Patil" },
                    { 2, "mgr@company.com", "$2a$11$mSeJEUK/usSIy52Lb7NjHu2M91Ll3327m16OM8hQcLG5IuCw.9B5K", "Manager", 0.00m, "Alex Mercer" },
                    { 3, "david@company.com", "$2a$11$mSeJEUK/usSIy52Lb7NjHu2M91Ll3327m16OM8hQcLG5IuCw.9B5K", "Employee", 38.20m, "David Miller" },
                    { 4, "sarah@company.com", "$2a$11$mSeJEUK/usSIy52Lb7NjHu2M91Ll3327m16OM8hQcLG5IuCw.9B5K", "Employee", 45.10m, "Sarah Jenkins" }
                });

            migrationBuilder.InsertData(
                table: "DailyMoodLogs",
                columns: new[] { "Id", "LogDate", "Score", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), 8, 1 },
                    { 2, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), 5, 3 },
                    { 3, new DateTime(2026, 5, 23, 0, 0, 0, 0, DateTimeKind.Utc), 9, 4 },
                    { 4, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), 7, 1 },
                    { 5, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), 4, 3 },
                    { 6, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), 8, 4 }
                });

            migrationBuilder.InsertData(
                table: "KudosCards",
                columns: new[] { "Id", "Message", "ReceiverId", "SenderId", "Timestamp" },
                values: new object[,]
                {
                    { 1, "@David saved my deployment pipeline at midnight! Incredible work.", 3, 1, new DateTime(2026, 5, 23, 6, 47, 12, 397, DateTimeKind.Utc).AddTicks(768) },
                    { 2, "@Sarah handled the enterprise API configuration perfectly. Thanks for unblocking!", 4, 3, new DateTime(2026, 5, 23, 9, 47, 12, 397, DateTimeKind.Utc).AddTicks(773) },
                    { 3, "Shoutout to @Sanjana for completing the complex DB scheme mapping ahead of schedule!", 1, 4, new DateTime(2026, 5, 23, 11, 17, 12, 397, DateTimeKind.Utc).AddTicks(777) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyMoodLog_UserId",
                table: "DailyMoodLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyMoodLogs_UserId_LogDate",
                table: "DailyMoodLogs",
                columns: new[] { "UserId", "LogDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KudosCard_ReceiverId",
                table: "KudosCards",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_KudosCard_SenderId",
                table: "KudosCards",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyMoodLogs");

            migrationBuilder.DropTable(
                name: "KudosCards");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
