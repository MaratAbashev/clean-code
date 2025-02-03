using MarkdownWebApi.Application.Assistants;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers.Handlers;

public static class ResultHandler
{
    public static IActionResult ShowActionResult(this ControllerBase controllerBase, Result result)
    {
        return result.StatusCode switch
        {
            0 => controllerBase.Ok(),
            200 => controllerBase.Ok(),
            400 => controllerBase.BadRequest(result.Error),
            401 => controllerBase.Unauthorized(result.Error),
            403 => controllerBase.Forbid(result.Error),
            404 => controllerBase.NotFound(result.Error),
            _ => controllerBase.StatusCode(result.StatusCode, result.Error)
        };
    }
    
    public static IActionResult ShowActionResult<T>(this ControllerBase controllerBase, Result<T> result)
    {
        return result.StatusCode switch
        {
            0 => controllerBase.Ok(result.Value),
            200 => controllerBase.Ok(result.Value),
            400 => controllerBase.BadRequest(result.Error),
            401 => controllerBase.Unauthorized(result.Error),
            403 => controllerBase.Forbid(result.Error),
            404 => controllerBase.NotFound(result.Error),
            _ => controllerBase.StatusCode(result.StatusCode, result.Error)
        };
    }
}