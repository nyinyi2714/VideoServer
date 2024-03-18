using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoModel.Migrations
{
    /// <inheritdoc />
    public partial class datetime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Video",
                newName: "Videos");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameIndex(
                name: "IX_Video_UserId",
                table: "Videos",
                newName: "IX_Videos_UserId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Timestamp",
                table: "Videos",
                type: "datetime2",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Videos",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Videos");

            migrationBuilder.RenameTable(
                name: "Videos",
                newName: "Video");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameIndex(
                name: "IX_Videos_UserId",
                table: "Video",
                newName: "IX_Video_UserId");

            migrationBuilder.AlterColumn<byte[]>(
                name: "Timestamp",
                table: "Video",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldRowVersion: true);
        }
    }
}
