using BaseLibrary.Entities;

namespace Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> UpdatePasswordAsync(string username, string newPassword);
    }
}
