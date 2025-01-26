using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Services.Interfaces;

namespace Server.Hubs
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public MessageHub(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task SendMessage(string content)
        {
            var username = Context.User.Identity?.Name;
            if (string.IsNullOrEmpty(username) || !Context.User.Identity.IsAuthenticated)
            {
                throw new HubException("User is not authenticated.");
            }

            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null) throw new HubException("User not found.");

            var messageDto = new CreateMessageDTO { Content = content };
            var newMessage = await _messageService.CreateMessageAsync(user.Id, messageDto);

            // Send the message to all connected clients
            await Clients.All.SendAsync("ReceiveMessage", newMessage);
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Caller.SendAsync("ReceiveMessage", new { Content = "Welcome to the chat!" });
            await base.OnConnectedAsync();
        }
    }
}
