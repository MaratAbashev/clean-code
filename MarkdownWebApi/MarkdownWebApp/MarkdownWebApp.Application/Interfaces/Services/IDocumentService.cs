using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Services;

public interface IDocumentService
{
    Task<Result<DocumentModel>> GetDocument(Guid documentId);
    Task<Result<string>> ChangeName(Guid documentId, string newDocumentName);
    Task<Result> DeleteDocument(Guid documentId);
    Task<Result<Guid>> CreateDocument(string documentName);
    Task<Result<string>> GetHtmlText(Guid documentId, string markdownText);
}