using System.Security.Claims;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Contracts.Accesses;
using MarkdownWebApi.Application.Contracts.Documents;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApi.Core.Models;
using MarkdownWebApp.Api.Controllers.Handlers;
using MarkdownWebApp.Api.Filters.AccessFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class DocumentAccessController(IDocumentAccessService documentAccessService): ControllerBase
{
    [HttpGet("/documents")]
    public async Task<IActionResult> GetDocuments()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        
        var documentsResult = await documentAccessService.GetAllDocumentsAsync(userId);
        return this.ShowActionResult(documentsResult);
    }

    [HttpPost("/documents/create")]
    public async Task<IActionResult> CreateDocument([FromBody] CreateDocumentRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var documentResult = await documentAccessService.CreateDocument(userId, request.DocumentName);
        return this.ShowActionResult(documentResult);
    }

    [HttpDelete("/documents/delete/")]
    [ServiceFilter(typeof(DeleteAccessFilter))]
    public async Task<IActionResult> DeleteDocumentAccess([FromBody] DeleteDocumentAccessRequest request)
    {
        var deleteResult = await documentAccessService.DeleteDocumentAccess(request.Email, request.DocumentId);
        return this.ShowActionResult(deleteResult);
    }

    [HttpPost("/documents/access/change")]
    [ServiceFilter(typeof(ChangeAccessFilter))]
    public async Task<IActionResult> ChangeAccess([FromBody] ChangeAccessLevelRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var changeAccessLevelResult = await documentAccessService.ChangeAccessLevel(userId, request.DocumentId, request.AccessLevel);
        return this.ShowActionResult(changeAccessLevelResult);
    }

    [HttpPost("/documents/access/allow")]
    [ServiceFilter(typeof(AllowAccessFilter))]
    public async Task<IActionResult> AllowAccess([FromBody] AllowAccessRequest request)
    {
        var allowAccessResult = await documentAccessService.AllowAccess(request.Email, request.DocumentId, request.Role);
        return this.ShowActionResult(allowAccessResult);
    }
}