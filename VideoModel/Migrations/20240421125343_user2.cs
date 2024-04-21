using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoModel.Migrations
{
    /// <inheritdoc />
    public partial class user2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_UserId",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Videos");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Videos_AspNetUsers",
                table: "Videos");

            migrationBuilder.DropIndex(
                name: "IX_Videos_VideoUserId",
                table: "Videos");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Videos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Videos_UserId",
                table: "Videos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Videos_AspNetUsers",
                table: "Videos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
