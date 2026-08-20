using System.ComponentModel.DataAnnotations;

namespace PharmacyManagement.Models
{
    public class ChatConversation
    {
        public long ConversationID { get; set; }

        public long UserID { get; set; }

        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsArchived { get; set; }

        public User User { get; set; } = null!;

        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
