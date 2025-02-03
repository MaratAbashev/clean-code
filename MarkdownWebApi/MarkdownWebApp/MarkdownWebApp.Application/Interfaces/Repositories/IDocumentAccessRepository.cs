using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Interfaces.Repositories;

public interface IDocumentAccessRepository
{
    Task<Result<List<DocumentAccessModel>>> GetAllDocumentAccesses(Guid userId);

    Task<Result<DocumentModel>> ChangeDocumentAccess(Guid userId, Guid documentId,
        AccessLevelModel accessLevelModel);
    Task<Result<DocumentAccessModel>> ControlAccess(string email, Guid documentId, RoleModel role);
    Task<Result<DocumentAccessModel>> CreateDocumentAccess(Guid userId, Guid documentId);
    Task<Result<Guid>> DeleteDocumentAccess(string email, Guid documentId);
    Task<Result<RoleModel>> GetUserRole(Guid userId, Guid documentId);
    Task<Result<DocumentAccessModel>> JoinByLink(Guid userId, Guid documentId);
}