using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task<Result<UserModel>> GetUserByEmail(string email);
    Task<Result> AddUserAsync(Guid userId, string username, string email, string password);
    Task<bool> UserExists(string email);
}