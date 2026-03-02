using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailAlertFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlertEmails",
                table: "MonitoredUrls",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SendDownAlert",
                table: "MonitoredUrls",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlertEmails",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "SendDownAlert",
                table: "MonitoredUrls");
        }
    }
}
