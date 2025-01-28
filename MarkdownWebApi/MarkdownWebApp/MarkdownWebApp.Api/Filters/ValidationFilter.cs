using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarkdownWebApp.Api.Filters;

public class ValidationFilter: Attribute, IAsyncActionFilter
{
    private readonly Type _validatorType;

    public ValidationFilter(Type validatorType)
    {
        if (!typeof(IValidator).IsAssignableFrom(validatorType))
        {
            throw new ArgumentException($"{validatorType} does not implement IValidator");
        }

        _validatorType = validatorType;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameter = context.ActionArguments.FirstOrDefault().Value;
        if (parameter == null)
        {
            context.Result = new BadRequestObjectResult("Model is null.");
            return;
        }

        if (context.HttpContext.RequestServices.GetService(_validatorType) is not IValidator validator)
        {
            context.Result = new BadRequestObjectResult("Validator not found.");
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