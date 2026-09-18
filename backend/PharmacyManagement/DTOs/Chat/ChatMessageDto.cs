namespace PharmacyManagement.DTOs.Chat
{
    public class ChatMessageDto
    {
        public long MessageId { get; set; }
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}