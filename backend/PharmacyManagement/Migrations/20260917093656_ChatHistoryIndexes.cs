using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyManagement.Migrations
{
    /// <inheritdoc />
    public partial class ChatHistoryIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ConversationID",
                table: "ChatMessage");

            migrationBuilder.DropIndex(
                name: "IX_ChatConversation_UserID",
                table: "ChatConversation");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ConversationID_MessageID",
                table: "ChatMessage",
                columns: new[] { "ConversationID", "MessageID" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversation_UserID_UpdatedAt",
                table: "ChatConversation",
                columns: new[] { "UserID", "UpdatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatMessage_ConversationID_MessageID",
                table: "ChatMessage");

            migrationBuilder.DropIndex(
                name: "IX_ChatConversation_UserID_UpdatedAt",
                table: "ChatConversation");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ConversationID",
                table: "ChatMessage",
                column: "ConversationID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversation_UserID",
                table: "ChatConversation",
                column: "UserID");
        }
    }
}
