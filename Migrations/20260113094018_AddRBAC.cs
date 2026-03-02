using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UrlHealthMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddRBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Companies_CompanyId",
                table: "AppUsers");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "AppUsers",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUsers_CompanyId",
                table: "AppUsers",
                newName: "IX_AppUsers_RoleId");

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "MonitoredUrls",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "AppUsers",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitoredUrls_AppUserId",
                table: "MonitoredUrls",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Role_RoleId",
                table: "AppUsers",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoredUrls_AppUsers_AppUserId",
                table: "MonitoredUrls",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUsers_Role_RoleId",
                table: "AppUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_MonitoredUrls_AppUsers_AppUserId",
                table: "MonitoredUrls");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropIndex(
                name: "IX_MonitoredUrls_AppUserId",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "MonitoredUrls");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "AppUsers");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "AppUsers",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_AppUsers_RoleId",
                table: "AppUsers",
                newName: "IX_AppUsers_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUsers_Companies_CompanyId",
                table: "AppUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
