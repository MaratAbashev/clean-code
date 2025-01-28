using FluentValidation;
using MarkdownWebApp.Api.Contracts.Users;

namespace MarkdownWebApi.Application;

public class LoginRequestValidator: AbstractValidator<LoginUserRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is required");
        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password is required");
    }
}