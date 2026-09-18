namespace PharmacyManagement.DTOs.Chat
{
    public class ChatConversationSummaryDto
    {
        public long ConversationId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastMessagePreview { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}