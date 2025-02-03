using MarkdownWebApi.Application.Assistants;

namespace MarkdownWebApi.Application.Interfaces.Services;

public interface IUserService
{
    Task<Result> Register(string username, string email, string password);
    Task<Result<string>> Login(string email, string password);
}