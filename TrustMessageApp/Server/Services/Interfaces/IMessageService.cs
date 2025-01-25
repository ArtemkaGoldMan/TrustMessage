using BaseLibrary.DTOs;

namespace Server.Services.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDTO> CreateMessageAsync(int userId, CreateMessageDTO messageDto);
        Task<IEnumerable<MessageDTO>> GetGeneralMessagesAsync();
        Task<IEnumerable<MessageDTO>> GetPersonalMessagesAsync(int userId);
    }
}
