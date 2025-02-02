using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<Result<Guid>> CreateDocument(string documentName);
    Task<Result<string>> ChangeName(Guid documentId, string newDocumentName);
    Task<Result> DeleteDocument(Guid documentId);
    Task<Result<DocumentModel>> GetDocument(Guid documentId);
}