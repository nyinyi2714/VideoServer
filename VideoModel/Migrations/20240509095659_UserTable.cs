using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoModel.Migrations
{
    /// <inheritdoc />
    public partial class UserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers",
                table: "Videos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Video_1",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "VideoUserId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Videos",
                type: "nvarchar(36)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Videos",
                table: "Videos",
                column: "VideoId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_Users_IdentityUserId",
                table: "Videos");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Videos",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_IdentityUserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "VideoUserId",
                table: "Videos",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Video_1",
                table: "Videos",
                column: "VideoId");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_VideoUserId",
                table: "Videos",
                column: "VideoUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers",
                table: "Videos",
                column: "VideoUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
