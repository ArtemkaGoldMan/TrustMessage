using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Server.Services.Interfaces;
using System.Security.Claims;
using Server.Hubs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/messages")]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageController(IMessageService messageService, IHubContext<MessageHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessages()
        {
            var messages = await _messageService.GetAllMessagesAsync();
            return Ok(messages);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetUserMessages()
        {
            var username = User.FindFirstValue(ClaimTypes.Name);
            var messages = await _messageService.GetUserMessagesAsync(username);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var username = User.FindFirstValue(ClaimTypes.Name);
            var message = await _messageService.CreateMessageAsync(username, request);

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.Username, message.Content);

            return Ok(message);
        }
    }
}
