using FluentValidation;
using MarkdownWebApi.Application.Contracts.Users;

namespace MarkdownWebApi.Application.Validators;

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