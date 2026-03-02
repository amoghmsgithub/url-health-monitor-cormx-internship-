using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryAckFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRecoveryAt",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastRecoveryRequestId",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastRecoveryStatus",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRecoveryAt",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "LastRecoveryRequestId",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "LastRecoveryStatus",
                table: "MonitoredUrls");
        }
    }
}
