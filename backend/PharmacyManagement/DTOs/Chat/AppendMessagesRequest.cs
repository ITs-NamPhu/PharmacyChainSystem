namespace PharmacyManagement.DTOs.Chat
{
    public class AppendMessagesRequest
    {
        public long? ConversationId { get; set; }
        public string UserMessage { get; set; } = string.Empty;
        public string AiMessage { get; set; } = string.Empty;
    }
}