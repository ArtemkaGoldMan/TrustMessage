using BaseLibrary.DTOs;

namespace Server.Services.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageDTO>> GetAllMessagesAsync();
        Task<IEnumerable<MessageDTO>> GetUserMessagesAsync(string username);
        Task<MessageDTO> CreateMessageAsync(string username, CreateMessageDTO request);
    }
}
