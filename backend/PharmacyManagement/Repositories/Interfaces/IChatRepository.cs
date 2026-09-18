using PharmacyManagement.Models;

namespace PharmacyManagement.Repositories.Interfaces
{
    public interface IChatRepository
    {
        Task<ChatConversation?> GetConversationAsync(long conversationId, long userId);
        Task<bool> IsConversationOwnerAsync(long conversationId, long userId);
        IQueryable<ChatConversation> GetConversationsQuery(long userId);
        IQueryable<ChatMessage> GetMessagesQuery(long conversationId);
        Task AddConversationAsync(ChatConversation conversation);
        Task AddMessageAsync(ChatMessage message);
        void UpdateConversation(ChatConversation conversation);
        Task SaveChangesAsync();
    }
}