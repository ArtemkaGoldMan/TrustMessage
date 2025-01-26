using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace WebClient.Services
{
    public class MessageService
    {
        private readonly string _webSocketUrl = "wss://localhost:7150/ws/messages";
        private ClientWebSocket _webSocket = new();

        public event Action<MessageDTO>? OnMessageReceived;

        public async Task ConnectAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(new Uri(_webSocketUrl), CancellationToken.None);
                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket connection error: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
                    break;
                }

                var jsonMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var message = JsonSerializer.Deserialize<MessageDTO>(jsonMessage);

                if (message != null)
                {
                    OnMessageReceived?.Invoke(message);
                }
            }
        }

        public async Task SendMessageAsync(string content)
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                var message = new { Content = content };
                var json = JsonSerializer.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                await _webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public async Task DisconnectAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
        }
    }
}
