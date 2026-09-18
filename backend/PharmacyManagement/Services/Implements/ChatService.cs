using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PharmacyManagement.DTOs.Chat;
using PharmacyManagement.Exceptions;
using PharmacyManagement.Models;
using PharmacyManagement.Repositories.Interfaces;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Services.Implements
{
    public class ChatService : IChatService
    {
        private const int DefaultMessageLimit = 20;
        private const int MaxMessageLimit = 50;
        private const int PreviewLength = 120;
        private const int TitleLength = 60;

        private readonly IChatRepository _repository;

        public ChatService(IChatRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ChatConversationSummaryDto>> GetConversationsAsync(long userId, long? beforeId, int limit)
        {
            if (limit <= 0 || limit > MaxMessageLimit) limit = DefaultMessageLimit;

            var query = _repository.GetConversationsQuery(userId);

            if (beforeId.HasValue)
            {
                query = query.Where(c => c.ConversationID < beforeId.Value);
            }

            var summaries = await query
                .OrderByDescending(c => c.UpdatedAt)
                .Take(limit)
                .Select(c => new ChatConversationSummaryDto
                {
                    ConversationId = c.ConversationID,
                    Title = c.Title,
                    UpdatedAt = c.UpdatedAt,
                    LastMessagePreview = c.Messages
                        .OrderByDescending(m => m.MessageID)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                    LastMessageAt = c.Messages
                        .OrderByDescending(m => m.MessageID)
                        .Select(m => m.CreatedAt)
                        .FirstOrDefault()
                })
                .ToListAsync();

            foreach (var item in summaries)
            {
                if (!string.IsNullOrEmpty(item.LastMessagePreview) && item.LastMessagePreview.Length > PreviewLength)
                {
                    item.LastMessagePreview = item.LastMessagePreview.Substring(0, PreviewLength) + "...";
                }
            }

            return summaries;
        }

        public async Task<long> CreateConversationAsync(long userId, string? firstMessage)
        {
            var conversation = new ChatConversation
            {
                UserID = userId,
                Title = ToTitle(firstMessage),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsArchived = false
            };

            await _repository.AddConversationAsync(conversation);
            await _repository.SaveChangesAsync();

            return conversation.ConversationID;
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(long userId, long conversationId, long? beforeId, int limit)
        {
            if (limit <= 0 || limit > MaxMessageLimit) limit = DefaultMessageLimit;

            if (!await _repository.IsConversationOwnerAsync(conversationId, userId))
            {
                throw new BusinessException("Conversation not found.", "CHAT001", StatusCodes.Status404NotFound);
            }

            var query = _repository.GetMessagesQuery(conversationId);

            if (beforeId.HasValue)
            {
                query = query.Where(m => m.MessageID < beforeId.Value);
            }

            // Lấy window tin gần nhất (DESC theo MessageID), sau đó đảo ngược để trả về cũ -> mới
            var messages = await query
                .OrderByDescending(m => m.MessageID)
                .Take(limit)
                .Select(m => new ChatMessageDto
                {
                    MessageId = m.MessageID,
                    Role = m.Role,
                    Content = m.Content,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            messages.Reverse();

            return messages;
        }

        public async Task<AppendMessagesResponse> AppendMessagesAsync(long userId, AppendMessagesRequest request)
        {
            ChatConversation conversation;

            if (request.ConversationId.HasValue && request.ConversationId.Value > 0)
            {
                var existing = await _repository.GetConversationAsync(request.ConversationId.Value, userId);
                if (existing == null)
                {
                    throw new BusinessException("Conversation not found.", "CHAT001", StatusCodes.Status404NotFound);
                }
                conversation = existing;
            }
            else
            {
                conversation = new ChatConversation
                {
                    UserID = userId,
                    Title = ToTitle(request.UserMessage),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsArchived = false
                };
                await _repository.AddConversationAsync(conversation);
                await _repository.SaveChangesAsync();
            }

            await _repository.AddMessageAsync(new ChatMessage
            {
                ConversationID = conversation.ConversationID,
                Role = "user",
                Content = request.UserMessage ?? string.Empty,
                CreatedAt = DateTime.Now
            });

            await _repository.AddMessageAsync(new ChatMessage
            {
                ConversationID = conversation.ConversationID,
                Role = "assistant",
                Content = request.AiMessage ?? string.Empty,
                CreatedAt = DateTime.Now
            });

            if (string.IsNullOrWhiteSpace(conversation.Title))
            {
                conversation.Title = ToTitle(request.UserMessage);
            }

            conversation.UpdatedAt = DateTime.Now;
            _repository.UpdateConversation(conversation);
            await _repository.SaveChangesAsync();

            return new AppendMessagesResponse
            {
                ConversationId = conversation.ConversationID,
                Title = conversation.Title
            };
        }

        private static string ToTitle(string? message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Cuộc trò chuyện mới";
            }

            var trimmed = message.Trim().Replace("\r", " ").Replace("\n", " ");
            return trimmed.Length <= TitleLength ? trimmed : trimmed.Substring(0, TitleLength) + "...";
        }
    }
}