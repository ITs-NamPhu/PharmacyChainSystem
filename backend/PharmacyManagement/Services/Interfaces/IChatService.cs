using PharmacyManagement.DTOs.Chat;

namespace PharmacyManagement.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatConversationSummaryDto>> GetConversationsAsync(long userId, long? beforeId, int limit);
        Task<long> CreateConversationAsync(long userId, string? firstMessage);
        Task<List<ChatMessageDto>> GetMessagesAsync(long userId, long conversationId, long? beforeId, int limit);
        Task<AppendMessagesResponse> AppendMessagesAsync(long userId, AppendMessagesRequest request);
    }
}