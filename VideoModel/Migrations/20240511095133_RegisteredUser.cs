using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoModel.Migrations
{
    /// <inheritdoc />
    public partial class RegisteredUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_IdentityUserId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Videos_IdentityUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Videos",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RegisteredUsers",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredUsers", x => x.Username);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_Username",
                table: "Videos",
                column: "Username");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_RegisteredUsers_Username",
                table: "Videos",
                column: "Username",
                principalTable: "RegisteredUsers",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_RegisteredUsers_Username",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "RegisteredUsers");

            migrationBuilder.DropIndex(
                name: "IX_Videos_Username",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Videos",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdentityUserId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdentityUserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Videos_IdentityUserId",
                table: "Videos",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_Users_IdentityUserId",
                table: "Videos",
                column: "IdentityUserId",
                principalTable: "Users",
                principalColumn: "IdentityUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
