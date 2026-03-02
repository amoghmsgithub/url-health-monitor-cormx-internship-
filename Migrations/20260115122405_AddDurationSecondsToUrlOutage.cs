using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationSecondsToUrlOutage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DownEndedAt",
                table: "UrlOutages",
                newName: "RecoveredAt");

            migrationBuilder.AddColumn<double>(
                name: "DurationSeconds",
                table: "UrlOutages",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "UrlOutages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationSeconds",
                table: "UrlOutages");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "UrlOutages");

            migrationBuilder.RenameColumn(
                name: "RecoveredAt",
                table: "UrlOutages",
                newName: "DownEndedAt");
        }
    }
}
