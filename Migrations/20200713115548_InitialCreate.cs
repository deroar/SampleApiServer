using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SampleApiServer.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerAuth",
                columns: table => new
                {
                    PlayerId = table.Column<long>(nullable: false),
                    DeviceId = table.Column<string>(maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    PlayerUidHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerAuth", x => new { x.PlayerId, x.DeviceId });
                });

            migrationBuilder.CreateTable(
                name: "PlayerBasics",
                columns: table => new
                {
                    PlayerId = table.Column<long>(nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(nullable: false),
                    Version = table.Column<long>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TutorialProgress = table.Column<int>(nullable: false),
                    Stamina = table.Column<int>(nullable: false),
                    LastStaminaUpdatedAt = table.Column<DateTimeOffset>(nullable: false),
                    LastLogin = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBasics", x => x.PlayerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBasics_CreatedAt",
                table: "PlayerBasics",
                column: "CreatedAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerAuth");

            migrationBuilder.DropTable(
                name: "PlayerBasics");
        }
    }
}
