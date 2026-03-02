using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddUrlOutages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UrlOutages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MonitoredUrlId = table.Column<int>(type: "INTEGER", nullable: false),
                    DownStartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DownEndedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UrlOutages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UrlOutages_MonitoredUrls_MonitoredUrlId",
                        column: x => x.MonitoredUrlId,
                        principalTable: "MonitoredUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UrlOutages_MonitoredUrlId",
                table: "UrlOutages",
                column: "MonitoredUrlId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UrlOutages");
        }
    }
}
