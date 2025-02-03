using System.Security.Claims;
using MarkdownWebApi.Application.Contracts.Accesses;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MarkdownWebApp.Api.Filters.AccessFilters;

public class AllowAccessFilter(IDocumentAccessRepository documentAccessRepository): IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionArguments.FirstOrDefault().Value is not AllowAccessRequest parameter)
        {
            context.Result = new BadRequestObjectResult("Model is null.");
            return;
        }
        var userId = Guid.Parse(context.HttpContext.User.Claims.FirstOrDefault(c => 
            c.Type == ClaimTypes.NameIdentifier)?.Value!);
        var documentAccessesResult = await documentAccessRepository.GetUserRole(userId, parameter.DocumentId);
        if (!documentAccessesResult.IsSuccess)
        {
            context.Result = new StatusCodeResult(documentAccessesResult.StatusCode);
            return;
        }

        if ((int)documentAccessesResult.Value > (int)RoleModel.Creator)
        {
            context.Result = new BadRequestObjectResult("You do not have permission to allow access to this document.");
            return;
        }
        await next();
    }
}