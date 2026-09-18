using Microsoft.EntityFrameworkCore;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;

namespace PharmacyManagement.Repositories.Implements
{
    public class ChatRepository : IChatRepository
    {
        private readonly PharmacySystemDbContext _context;

        public ChatRepository(PharmacySystemDbContext context)
        {
            _context = context;
        }

        public async Task<ChatConversation?> GetConversationAsync(long conversationId, long userId)
        {
            return await _context.ChatConversation
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId && c.UserID == userId);
        }

        public Task<bool> IsConversationOwnerAsync(long conversationId, long userId)
        {
            return _context.ChatConversation
                .AsNoTracking()
                .AnyAsync(c => c.ConversationID == conversationId && c.UserID == userId);
        }

        public IQueryable<ChatConversation> GetConversationsQuery(long userId)
        {
            return _context.ChatConversation
                .AsNoTracking()
                .Where(c => c.UserID == userId && !c.IsArchived);
        }

        public IQueryable<ChatMessage> GetMessagesQuery(long conversationId)
        {
            return _context.ChatMessage
                .AsNoTracking()
                .Where(m => m.ConversationID == conversationId);
        }

        public async Task AddConversationAsync(ChatConversation conversation)
        {
            await _context.ChatConversation.AddAsync(conversation);
        }

        public async Task AddMessageAsync(ChatMessage message)
        {
            await _context.ChatMessage.AddAsync(message);
        }

        public void UpdateConversation(ChatConversation conversation)
        {
            _context.ChatConversation.Update(conversation);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}