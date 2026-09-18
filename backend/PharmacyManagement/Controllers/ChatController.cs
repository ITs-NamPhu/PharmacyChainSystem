using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.DTOs.Chat;
using PharmacyManagement.Services.Interfaces;

namespace PharmacyManagement.Controllers
{
    [Route("api/chat")]
    [ApiController]
    [Authorize]
    public class ChatController : BaseController
    {
        private const int DefaultLimit = 20;

        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations([FromQuery] long? beforeId, [FromQuery] int? limit)
        {
            var userId = GetUserIdFromToken();
            var result = await _chatService.GetConversationsAsync(userId, beforeId, limit ?? DefaultLimit);
            return Success(result, "Get conversations successfully.");
        }

        [HttpPost("conversations")]
        public async Task<IActionResult> CreateConversation([FromBody] CreateConversationRequest? request)
        {
            var userId = GetUserIdFromToken();
            var conversationId = await _chatService.CreateConversationAsync(userId, request?.FirstMessage);
            return Success(new { conversationId }, "Create conversation successfully.");
        }

        [HttpGet("conversations/{id}/messages")]
        public async Task<IActionResult> GetMessages(long id, [FromQuery] long? beforeId, [FromQuery] int? limit)
        {
            var userId = GetUserIdFromToken();
            var result = await _chatService.GetMessagesAsync(userId, id, beforeId, limit ?? DefaultLimit);
            return Success(result, "Get messages successfully.");
        }

        [HttpPost("messages")]
        public async Task<IActionResult> AppendMessages([FromBody] AppendMessagesRequest request)
        {
            var userId = GetUserIdFromToken();
            var result = await _chatService.AppendMessagesAsync(userId, request);
            return Success(result, "Messages saved successfully.");
        }
    }
}