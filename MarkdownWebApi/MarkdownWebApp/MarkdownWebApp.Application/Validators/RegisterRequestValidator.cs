using FluentValidation;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApp.Api.Contracts.Users;

namespace MarkdownWebApi.Application.Validators;

public class RegisterRequestValidator: AbstractValidator<RegisterUserRequest>
{
    public RegisterRequestValidator(IUserRepository userRepository)
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Email address is required")
            .MustAsync(async (email, _) => !await userRepository.UserExists(email))
            .WithMessage("This email address is already taken");
        RuleFor(r => r.UserName)
            .NotEmpty()
            .WithMessage("Username is required")
            .Matches("^[a-zA-Z0-9_]{5,20}")
            .WithMessage("Username must be between 5 and 20 characters");
        RuleFor(r => r.Password)
            .NotEmpty()
            .WithMessage("Password is required")
            .Matches(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{12,20}$")
            .WithMessage("Password must be 5 to 20 characters long, contain at least 1 latin capital letter, case letter, one number and one special character.");
    }
}