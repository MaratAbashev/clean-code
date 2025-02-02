using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DocumentAccessController: ControllerBase
{
    
}