using System.ComponentModel.DataAnnotations;

namespace MarkdownWebApp.Api.Contracts.Users;

public record RegisterUserRequest(string UserName, string Email, string Password);