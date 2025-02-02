using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Application.Interfaces.Repositories;

namespace MarkdownWebApi.Application.Services;

public class UserService(IPasswordHashier passwordHashier, IUserRepository userRepository, IJwtWorker jwtWorker)
    : IUserService
{
    public async Task<Result> Register(string username, string email, string password)
    {
        return await userRepository.AddUserAsync(Guid.NewGuid(), username, email, passwordHashier.HashPassword(password));
    }

    public async Task<Result<string>> Login(string email, string password)
    {
        var userResult = await userRepository.GetUserByEmail(email);
        if (!userResult.IsSuccess)
            return Result<string>.Fail(userResult.Error, userResult.StatusCode);
        if (!passwordHashier.VerifyHashedPassword(userResult.Value!.PasswordHash!, password))
            return Result<string>.Fail("Incorrect password", 401);
        return Result<string>.Ok(jwtWorker.GenerateJwtToken(userResult.Value!));
    }
}