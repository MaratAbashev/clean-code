using MarkdownWebApi.Application;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Contracts.Users;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApp.Api.Controllers.Handlers;
using MarkdownWebApp.Api.Filters.UserFilters;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [Route("/register")]
    [ServiceFilter(typeof(RegisterValidationFilter))]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        var result = await userService.Register(request.UserName, request.Email, request.Password);
        return this.ShowActionResult(result);
    }

    [HttpPost]
    [Route("/login")]
    [ServiceFilter(typeof(LoginValidationFilter))]
    public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
    {
        var tokenResult = await userService.Login(request.Email, request.Password);
        var serializedTokenResult = tokenResult.IsSuccess? 
            Result<object>.Ok(new{token = tokenResult.Value}):
            Result<object>.Fail(tokenResult.Error, tokenResult.StatusCode);
        HttpContext.Response.Cookies.Append("tasty-cookies", tokenResult.Value!);
        return this.ShowActionResult(serializedTokenResult);
    }
}