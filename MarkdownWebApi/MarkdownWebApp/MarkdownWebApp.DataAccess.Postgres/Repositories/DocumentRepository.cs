using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Core.Models;
using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkdownWebApp.DataAccess.Postgres.Repositories;

public class DocumentRepository(MarkdownDbContext context) : IDocumentRepository
{
    public async Task<Result<Guid>> CreateDocument(string documentName)
    {
        try
        {
            var id = Guid.NewGuid();
            var document = new DocumentEntity
            {
                Id = id,
                Name = documentName,
                Path = $"{documentName}_{id}"
            };
            await context.Documents.AddAsync(document);
            await context.SaveChangesAsync();
            return Result<Guid>.Ok(id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }

    public async Task<Result<string>> ChangeName(Guid documentId, string newDocumentName)
    {
        try
        {
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<string>.Fail("Document not found", 404);
            document.Name = newDocumentName;
            await context.SaveChangesAsync();
            return Result<string>.Ok(newDocumentName);
        }
        catch (Exception ex)
        {
            return Result<string>.FromException(ex, 500);
        }
    }

    public async Task<Result> DeleteDocument(Guid documentId)
    {
        try
        {
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<string>.Fail("Document not found", 404);
            await context.Documents
                .Where(d => d.Id == documentId)
                .ExecuteDeleteAsync();
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.FromException(ex, 500);
        }
    }

    public async Task<Result<DocumentModel>> GetDocument(Guid documentId)
    {
        try
        {
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<DocumentModel>.Fail("Document not found", 404);
            var documentModel = new DocumentModel()
            {
                DocumentId = document.Id,
                DocumentName = document.Name,
                AccessLevel = Enum.Parse<AccessLevelModel>(document.AccessLevel.ToString())
            };
            return Result<DocumentModel>.Ok(documentModel);
        }
        catch (Exception ex)
        {
            return Result<DocumentModel>.FromException(ex, 500);
        }
    }
}