using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Application.Interfaces.Services;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Services;

public class DocumentService(IDocumentRepository documentRepository, IMinioService minioService) : IDocumentService
{
    public async Task<Result<DocumentModel>> GetDocument(Guid documentId)
    {
        return await documentRepository.GetDocument(documentId);
    }

    public async Task<Result<string>> ChangeName(Guid documentId, string newDocumentName)
    {
        return await documentRepository.ChangeName(documentId, newDocumentName);
    }

    public async Task<Result> DeleteDocument(Guid documentId)
    {
        var deleteResult = await documentRepository.DeleteDocument(documentId);
        if (!deleteResult.IsSuccess)
            return deleteResult;
        try
        {
            return await minioService.DeleteDocument(documentId);
        }
        catch (Exception ex)
        {
            return Result.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> CreateDocument(string documentName)
    {
        var creationResult = await documentRepository.CreateDocument(documentName);
        if (!creationResult.IsSuccess)
            return creationResult;
        try
        {
            var creationFileResult = await minioService.CreateDocument(creationResult.Value);
            if (!creationFileResult.IsSuccess)
                return Result<Guid>.Fail(creationFileResult.Error, creationFileResult.StatusCode);
            return creationResult;
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }
}