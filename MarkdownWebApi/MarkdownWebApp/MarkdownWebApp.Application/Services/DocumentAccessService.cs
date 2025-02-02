using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Services;

public class DocumentAccessService(IDocumentAccessRepository documentAccessRepository, IDocumentService documentService) : IDocumentAccessService
{
    public async Task<Result<List<DocumentAccessModel>>> GetAllDocumentsAsync(Guid userId)
    {
        return await documentAccessRepository.GetAllDocumentAccesses(userId);
    }

    public async Task<Result<DocumentModel>> ChangeAccessLevel(Guid userId, Guid documentId, AccessLevelModel accessLevel)
    {
        return await documentAccessRepository.ChangeDocumentAccess(userId, documentId, accessLevel);
    }

    public async Task<Result<DocumentAccessModel>> CreateDocument(Guid userId, string documentName)
    {
        var documentCreationResult = await documentService.CreateDocument(documentName);
        if (!documentCreationResult.IsSuccess)
            return Result<DocumentAccessModel>.Fail(documentCreationResult.Error, documentCreationResult.StatusCode);
        return await documentAccessRepository.CreateDocumentAccess(userId, documentCreationResult.Value);
    }

    public async Task<Result<Guid>> DeleteDocumentAccess(string email, Guid documentId)
    {
        return await documentAccessRepository.DeleteDocumentAccess(email, documentId);
    }

    public async Task<Result<DocumentAccessModel>> AllowAccess(string email, Guid documentId, RoleModel role)
    {
        return await documentAccessRepository.AllowAccess(email, documentId, role);
    }
}