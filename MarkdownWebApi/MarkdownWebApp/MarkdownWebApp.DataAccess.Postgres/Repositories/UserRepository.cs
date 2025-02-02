using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Core.Models;
using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkdownWebApp.DataAccess.Postgres.Repositories;

public class UserRepository(MarkdownDbContext context) : IUserRepository
{
    public async Task<Result<UserModel>> GetUserByEmail(string email)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
        
        return user != null? 
            Result<UserModel>.Ok(new UserModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PasswordHash = user.Password
            })
            : Result<UserModel>.Fail("User not found", 404);
    }

    public async Task<Result> AddUserAsync(Guid userId, string username, string email, string password)
    {
        try
        {
            if (await context.Users.AnyAsync(u => u.Email == email))
                return Result.Fail("Email already exists", 409);
            var userEntity = new UserEntity()
            {
                Id = userId,
                Email = email,
                UserName = username,
                Password = password
            };
            context.Users.Add(userEntity);
            await context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.FromException(e, 500);
        }

    }

    public async Task<bool> UserExists(string email)
    {
        return await context.Users.AnyAsync(u => u.Email == email);
    }
}