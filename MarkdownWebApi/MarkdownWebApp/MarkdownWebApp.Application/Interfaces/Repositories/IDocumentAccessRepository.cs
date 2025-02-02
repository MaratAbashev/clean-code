using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Repositories;

public interface IDocumentAccessRepository
{
    Task<Result<List<DocumentAccessModel>>> GetAllDocumentAccesses(Guid userId);
    Task<Result<Guid>> ShareDocument(Guid userId, Guid documentId, bool makePublic);
    Task<Result<Guid>> AllowAccess(string email, Guid documentId, string role);
    Task<Result<Guid>> CreateDocumentAccess(Guid userId, Guid documentId);
    Task<Result<Guid>> DeleteDocumentAccess(Guid userId, Guid documentId);
}