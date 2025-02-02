using System.ComponentModel.DataAnnotations;

namespace MarkdownWebApp.Api.Contracts.Users;

public record LoginUserRequest(string Email, string Password);