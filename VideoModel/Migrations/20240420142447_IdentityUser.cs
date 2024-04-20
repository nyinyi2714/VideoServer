using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoModel.Migrations
{
    /// <inheritdoc />
    public partial class IdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Likes",
                table: "Videos",
                newName: "Views");

            migrationBuilder.CreateTable(
                name: "ChatRooms",
                columns: table => new
                {
                    ChatRoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ChatRoom__69733CF70BA3BE0E", x => x.ChatRoomId);
                });

            migrationBuilder.CreateTable(
                name: "ChatRoomUser",
                columns: table => new
                {
                    ChatRoomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatRoomUser", x => new { x.ChatRoomId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: false),
                    SenderId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Messages__4808B993CF9DC4C0", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_Messages_ChatRooms",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId");
                    table.ForeignKey(
                        name: "FK_Messages_Users",
                        column: x => x.SenderId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UsersChatRooms",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ChatRoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersChatRooms", x => new { x.UserId, x.ChatRoomId });
                    table.ForeignKey(
                        name: "FK_UsersChatRooms_ChatRooms",
                        column: x => x.ChatRoomId,
                        principalTable: "ChatRooms",
                        principalColumn: "ChatRoomId");
                    table.ForeignKey(
                        name: "FK_UsersChatRooms_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatRoomId",
                table: "Messages",
                column: "ChatRoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersChatRooms_ChatRoomId",
                table: "UsersChatRooms",
                column: "ChatRoomId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatRoomUser");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UsersChatRooms");

            migrationBuilder.DropTable(
                name: "ChatRooms");

            migrationBuilder.RenameColumn(
                name: "Views",
                table: "Videos",
                newName: "Likes");
        }
    }
}
