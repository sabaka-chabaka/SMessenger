using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SMessenger.ChatService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialChats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHATS",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHATS", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "USER_PUBLIC_KEYS",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    public_key_base64 = table.Column<string>(type: "text", nullable: false),
                    algorithm = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER_PUBLIC_KEYS", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "CHAT_MEMBERS",
                columns: table => new
                {
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_MEMBERS", x => new { x.chat_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_CHAT_MEMBERS_CHATS_chat_id",
                        column: x => x.chat_id,
                        principalTable: "CHATS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MESSAGES",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sender_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ciphertext_base64 = table.Column<string>(type: "text", nullable: false),
                    nonce = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    is_edited = table.Column<bool>(type: "boolean", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MESSAGES", x => x.id);
                    table.ForeignKey(
                        name: "FK_MESSAGES_CHATS_chat_id",
                        column: x => x.chat_id,
                        principalTable: "CHATS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CHAT_ENCRYPTED_KEYS",
                columns: table => new
                {
                    chat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    encrypted_key_base64 = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAT_ENCRYPTED_KEYS", x => new { x.chat_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_CHAT_ENCRYPTED_KEYS_CHATS_chat_id",
                        column: x => x.chat_id,
                        principalTable: "CHATS",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CHAT_ENCRYPTED_KEYS_USER_PUBLIC_KEYS_user_id",
                        column: x => x.user_id,
                        principalTable: "USER_PUBLIC_KEYS",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CHAT_ENCRYPTED_KEYS_user_id",
                table: "CHAT_ENCRYPTED_KEYS",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_MESSAGES_chat_id",
                table: "MESSAGES",
                column: "chat_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CHAT_ENCRYPTED_KEYS");

            migrationBuilder.DropTable(
                name: "CHAT_MEMBERS");

            migrationBuilder.DropTable(
                name: "MESSAGES");

            migrationBuilder.DropTable(
                name: "USER_PUBLIC_KEYS");

            migrationBuilder.DropTable(
                name: "CHATS");
        }
    }
}
