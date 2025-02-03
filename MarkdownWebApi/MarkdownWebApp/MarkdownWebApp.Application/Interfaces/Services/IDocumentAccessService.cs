using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Services;

public interface IDocumentAccessService
{
    Task<Result<List<DocumentAccessModel>>> GetAllDocumentsAsync(Guid userId);
    Task<Result<DocumentModel>> ChangeAccessLevel(Guid userId, Guid documentId, AccessLevelModel accessLevel);
    Task<Result<DocumentAccessModel>> CreateDocument(Guid userId, string documentName);
    Task<Result<Guid>> DeleteDocumentAccess(string email, Guid documentId);
    Task<Result<DocumentAccessModel>> AllowAccess(string email, Guid documentId, RoleModel role);
    Task<Result<DocumentAccessModel>> JoinByLink(Guid userId, Guid documentId);
}