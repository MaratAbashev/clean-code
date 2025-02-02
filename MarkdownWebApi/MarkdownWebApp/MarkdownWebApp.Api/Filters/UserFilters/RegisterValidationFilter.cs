using FluentValidation;
using MarkdownWebApi.Application.Contracts.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarkdownWebApp.Api.Filters.UserFilters;

public class RegisterValidationFilter(IValidator<RegisterUserRequest> validator) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.FirstOrDefault().Value is not RegisterUserRequest parameter)
        {
            context.Result = new BadRequestObjectResult("Model is null.");
            return;
        }

        var validationContext = new ValidationContext<object>(parameter);
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