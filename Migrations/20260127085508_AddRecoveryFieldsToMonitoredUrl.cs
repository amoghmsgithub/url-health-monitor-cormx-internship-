using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddRecoveryFieldsToMonitoredUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppPoolName",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RecoveryAction",
                table: "MonitoredUrls",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecoveryWebsiteUrl",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppPoolName",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "RecoveryAction",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "RecoveryWebsiteUrl",
                table: "MonitoredUrls");
        }
    }
}
