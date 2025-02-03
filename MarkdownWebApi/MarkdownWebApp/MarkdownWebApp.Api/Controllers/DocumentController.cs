using System.Security.Claims;
using System.Text;
using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Contracts.Documents;
using MarkdownWebApi.Application.Dto;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApp.Api.Controllers.Handlers;
using MarkdownWebApp.Api.Filters.DocumentFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarkdownWebApp.Api.Controllers;

[ApiController]
[Route("/document")]
[Authorize]
public class DocumentController(IDocumentService documentService): ControllerBase
{
    [HttpGet("/{documentId:guid}")]
    [ServiceFilter(typeof(GetDocumentFilter))]
    public async Task<IActionResult> GetDocument([FromRoute] Guid documentId, [FromServices] IDocumentAccessService documentAccessService, [FromServices] IMinioService minioService)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var getDocumentResult = await documentAccessService.JoinByLink(userId, documentId);
        if (!getDocumentResult.IsSuccess)
            return this.ShowActionResult(getDocumentResult);
        var documentTextResult = await minioService.PullDocument(getDocumentResult.Value!.DocumentId);
        if (!documentTextResult.IsSuccess)
            return this.ShowActionResult(documentTextResult);
        var convertedTextResult = await documentService.GetHtmlText(getDocumentResult.Value!.DocumentId, documentTextResult.Value!);
        if (!convertedTextResult.IsSuccess)
            return this.ShowActionResult(convertedTextResult);
        var documentDtoResult = Result<DocumentContentDto>.Ok(new DocumentContentDto()
        {
            Title = getDocumentResult.Value.DocumentName,
            Text = documentTextResult.Value!,
            ConvertedText = convertedTextResult.Value!
        });
        return this.ShowActionResult(documentDtoResult);
    }

    [HttpDelete("/{documentId:guid}")]
    [ServiceFilter(typeof(DeleteDocumentFilter))]
    public async Task<IActionResult> DeleteDocument([FromRoute] Guid documentId)
    {
        var deleteDocumentResult = await documentService.DeleteDocument(documentId);
        return this.ShowActionResult(deleteDocumentResult);
    }

    [HttpPost("/{documentId:guid}/rename/{newDocumentName}")]
    [ServiceFilter(typeof(RenameDocumentFilter))]
    public async Task<IActionResult> RenameDocument([FromRoute] Guid documentId, [FromRoute] string newDocumentName)
    {
        var renameDocumentResult = await documentService.ChangeName(documentId, newDocumentName);
        return this.ShowActionResult(renameDocumentResult);
    }

    [HttpPost("/{documentId:guid}/edit")]
    [ServiceFilter(typeof(EditDocumentFilter))]
    public async Task<IActionResult> EditDocument([FromRoute] Guid documentId, [FromBody] EditDocumentRequest editDocumentRequest, [FromServices] IMinioService minioService)
    {
        var getDocumentResult = await documentService.GetDocument(documentId);
        if (!getDocumentResult.IsSuccess)
            return this.ShowActionResult(getDocumentResult);
        var parseResult = await documentService.GetHtmlText(documentId, editDocumentRequest.Content);
        if (!parseResult.IsSuccess)
            return this.ShowActionResult(parseResult);
        var pushResult = await minioService.PushDocument(documentId, editDocumentRequest.Content);
        if (!pushResult.IsSuccess)
            return this.ShowActionResult(pushResult);
        var documentDtoResult = Result<DocumentContentDto>.Ok(new DocumentContentDto
        {
            Title = getDocumentResult.Value!.DocumentName,
            ConvertedText = parseResult.Value!,
            Text = editDocumentRequest.Content
        });
        return this.ShowActionResult(documentDtoResult);
    }

    [HttpPost("/{documentId:guid}/download/html")]
    [ServiceFilter(typeof(GetDocumentFilter))]
    public async Task<IActionResult> DownloadHtmlDocument([FromRoute] Guid documentId, [FromServices] IMinioService minioService)
    {
        var getDocumentResult = await documentService.GetDocument(documentId);
        if (!getDocumentResult.IsSuccess)
            return this.ShowActionResult(getDocumentResult);
        var getDocumentContentResult = await minioService.PullDocument(documentId);
        if (!getDocumentContentResult.IsSuccess)
            return this.ShowActionResult(getDocumentContentResult);
        var getParsedDocumentContentResult = await documentService.GetHtmlText(getDocumentResult.Value!.DocumentId, getDocumentContentResult.Value!);
        if (!getParsedDocumentContentResult.IsSuccess)
            return this.ShowActionResult(getParsedDocumentContentResult);
        
        return File(Encoding.UTF8.GetBytes(getParsedDocumentContentResult.Value!), "text/html", $"{getDocumentResult.Value!.DocumentName}.html");
    }
    
    [HttpPost("/{documentId:guid}/download/md")]
    [ServiceFilter(typeof(GetDocumentFilter))]
    public async Task<IActionResult> DownloadMdDocument([FromRoute] Guid documentId, [FromServices] IMinioService minioService)
    {
        var getDocumentResult = await documentService.GetDocument(documentId);
        if (!getDocumentResult.IsSuccess)
            return this.ShowActionResult(getDocumentResult);
        var getDocumentContentResult = await minioService.PullDocument(documentId);
        if (!getDocumentContentResult.IsSuccess)
            return this.ShowActionResult(getDocumentContentResult);
        return File(Encoding.UTF8.GetBytes(getDocumentContentResult.Value!), "text/plain", $"{getDocumentResult.Value!.DocumentName}.html");
    }
}