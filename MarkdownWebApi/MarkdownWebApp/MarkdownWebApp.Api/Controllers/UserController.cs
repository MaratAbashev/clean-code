using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;
using MarkdownWebApp.Api.Contracts.Users;
using MarkdownWebApp.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    [Route("register")]
    [ValidationFilter(typeof(RegisterRequestValidator))]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await _userService.Register(request.UserName, request.Email, request.Password);
        return ShowActionResult(result);
    }

    private IActionResult ShowActionResult(Result result)
    {
        return result.StatusCode switch
        {
            0 => Ok(),
            200 => Ok(),
            400 => BadRequest(result.Error),
            401 => Unauthorized(result.Error),
            403 => Forbid(result.Error),
            404 => NotFound(result.Error),
            _ => StatusCode(result.StatusCode, result.Error)
        };
    }
    
    private IActionResult ShowActionResult<T>(Result<T> result)
    {
        return result.StatusCode switch
        {
            0 => Ok(result.Value),
            200 => Ok(result.Value),
            400 => BadRequest(result.Error),
            401 => Unauthorized(result.Error),
            403 => Forbid(result.Error),
            404 => NotFound(result.Error),
            _ => StatusCode(result.StatusCode, result.Error)
        };
    }

    [HttpPost]
    [Route("login")]
    [ValidationFilter(typeof(LoginRequestValidator))]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request, HttpContext context)
    {
        var tokenResult = await _userService.Login(request.Email, request.Password);
        var serializedTokenResult = tokenResult.IsSuccess? 
            Result<object>.Ok(new{token = tokenResult.Value}):
            Result<object>.Fail(tokenResult.Error, tokenResult.StatusCode);
        return ShowActionResult<object>(serializedTokenResult);
    }
}