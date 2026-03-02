using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryCallbackStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RecoveryCompletedAt",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoveryMessage",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoveryStatus",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecoveryCompletedAt",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "RecoveryMessage",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "RecoveryStatus",
                table: "MonitoredUrls");
        }
    }
}
