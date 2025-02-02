using FluentValidation;
using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Validators;
using MarkdownWebApp.Api.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarkdownWebApp.Api.Filters;

public class LoginValidationFilter(IValidator<LoginUserRequest> validator) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.FirstOrDefault().Value is not LoginUserRequest parameter)
        {
            context.Result = new BadRequestObjectResult("Model is null.");
            return;
        }

        var validationContext = new ValidationContext<LoginUserRequest>(parameter);
        var validationResult = await validator.ValidateAsync(validationContext);

        if (!validationResult.IsValid)
        {
            context.Result = new BadRequestObjectResult(validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray()));
            return;
        }

        await next();
    }
}