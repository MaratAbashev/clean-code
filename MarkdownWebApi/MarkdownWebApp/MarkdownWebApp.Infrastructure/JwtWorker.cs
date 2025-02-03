using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MarkdownWebApi.Application.Interfaces.Auth;
using MarkdownWebApi.Core.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MarkdownWebApi.Infrastructure;

public class JwtWorker(IOptions<JwtOptions> options) : IJwtWorker
{
    private readonly JwtOptions _jwtOptions = options.Value;

    public string GenerateJwtToken(UserModel user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}