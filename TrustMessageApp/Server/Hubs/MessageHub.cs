using Microsoft.AspNetCore.SignalR;

namespace Server.Hubs
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string username, string content)
        {
            await Clients.All.SendAsync("ReceiveMessage", username, content);
        }
    }
}
