using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class ChatMessage
    {
        public long MessageID { get; set; }

        public long ConversationID { get; set; }

        [StringLength(20)]
        public string Role { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public ChatConversation Conversation { get; set; } = null!;
    }
}
