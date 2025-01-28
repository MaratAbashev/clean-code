using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Auth;

public interface IJwtWorker
{
    string GenerateJwtToken(UserModel user);
}