using FluentValidation;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Application.Interfaces.Repositories;


namespace MarkdownWebApi.Application;

public class UserService : IUserService
{
    private readonly IPasswordHashier _passwordHashier;
    private readonly IUserRepository _userRepository;
    private readonly IJwtWorker _jwtWorker;
    public UserService(IPasswordHashier passwordHashier, IUserRepository userRepository, IJwtWorker jwtWorker)
    {
        _passwordHashier = passwordHashier;
        _userRepository = userRepository;
        _jwtWorker = jwtWorker;
    }

    public async Task<Result> Register(string username, string email, string password)
    {
        return await _userRepository.AddUserAsync(Guid.NewGuid(), username, email, _passwordHashier.HashPassword(password));
    }

    public async Task<Result<string>> Login(string email, string password)
    {
        var userResult = await _userRepository.GetUserByEmail(email);
        if (!userResult.IsSuccess)
            return Result<string>.Fail(userResult.Error, userResult.StatusCode);
        if (!_passwordHashier.VerifyHashedPassword(userResult.Value!.PasswordHash, password))
            return Result<string>.Fail("Incorrect password", 401);
        return Result<string>.Ok(_jwtWorker.GenerateJwtToken(userResult.Value!));
    }
}