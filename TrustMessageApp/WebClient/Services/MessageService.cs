using BaseLibrary.DTOs;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace WebClient.Services
{
    public class MessageService
    {
        private readonly string _webSocketUrl = "wss://localhost:7150/ws/messages";
        private ClientWebSocket _webSocket = new();
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly HttpClient _httpClient;

        public event Action<MessageDTO>? OnMessageReceived;
        public event Action<string>? OnError;
        public event Action? OnDisconnected;

        public MessageService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task ConnectAsync()
        {
            if (_webSocket.State == WebSocketState.Open)
                return;

            _webSocket = new ClientWebSocket();

            // Add cookies to the WebSocket request
            var cookies = _httpClient.DefaultRequestHeaders.GetValues("Cookie").FirstOrDefault();
            if (!string.IsNullOrEmpty(cookies))
            {
                _webSocket.Options.SetRequestHeader("Cookie", cookies);
            }

            try
            {
                await _webSocket.ConnectAsync(new Uri(_webSocketUrl), _cancellationTokenSource.Token);
                _ = ReceiveMessagesAsync();
            }
            catch (Exception ex)
            {
                OnError?.Invoke($"WebSocket connection error: {ex.Message}");
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            var buffer = new byte[1024 * 4];

            try
            {
                while (_webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        OnDisconnected?.Invoke();
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
            catch (Exception ex)
            {
                OnError?.Invoke($"WebSocket receive error: {ex.Message}");
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
            else
            {
                OnError?.Invoke("WebSocket connection is not open.");
            }
        }

        public async Task DisconnectAsync()
        {
            _cancellationTokenSource.Cancel();
            if (_webSocket.State == WebSocketState.Open)
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            }
            OnDisconnected?.Invoke();
        }
    }
}
