using MarkdownWebApi.Application.Assistants;
using MarkdownWebApi.Application.Interfaces.Repositories;
using MarkdownWebApi.Core.Models;
using MarkdownWebApp.DataAccess.Postgres.Models;
using Microsoft.EntityFrameworkCore;

namespace MarkdownWebApp.DataAccess.Postgres.Repositories;

public class DocumentAccessRepository(MarkdownDbContext context) : IDocumentAccessRepository
{
    public async Task<Result<List<DocumentAccessModel>>> GetAllDocumentAccesses(Guid userId)
    {
        try
        {
            var documentAccessList = await context.DocumentAccesses
                .AsNoTracking()
                .Include(da => da.Document)
                .Where(da => da.UserId == userId && (da.Role == Role.Creator || da.Document!.AccessLevel != AccessLevel.Private))
                .Select(da => new DocumentAccessModel
                {
                    UserId = da.UserId,
                    Role = da.Role.ToString(),
                    DocumentName = da.Document!.Name,
                    DocumentId = da.DocumentId,
                    Users = context.DocumentAccesses.Where(docAcc => da.DocumentId == docAcc.DocumentId).AsNoTracking().Select(u => new UserModel()).ToList()
                })
                .ToListAsync();
            return Result<List<DocumentAccessModel>>.Ok(documentAccessList);
        }
        catch (Exception ex)
        {
            return Result<List<DocumentAccessModel>>.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> ShareDocument(Guid userId, Guid documentId, bool makePublic)
    {
        try
        {
            var query = context.DocumentAccesses
                .Where(da => da.DocumentId == documentId);
            if (!await query.AnyAsync())
                return Result<Guid>.Fail("Document not found", 404);
            query = query.Where(da => da.UserId == userId);
            if (!await query.AnyAsync())
                return Result<Guid>.Fail("You have no access to this document", 403);
            query = query.Where(da => da.Role == Role.Creator);
            if (!await query.AnyAsync())
                return Result<Guid>.Fail("Only creator can share this document", 403);
            await query
                .Include(da => da.Document)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(da => da.Document!.AccessLevel, makePublic? AccessLevel.Public: AccessLevel.OnLink));
            return Result<Guid>.Ok(documentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> AllowAccess(string email, Guid documentId, string role)
    {
        try
        {
            if (Enum.TryParse(role, out Role roleEnum))
                return Result<Guid>.Fail("Invalid role naming", 400);
            if (roleEnum == Role.Creator)
                return Result<Guid>.Fail("You can't give a creator role", 400);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Result<Guid>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<Guid>.Fail("Document not found", 404);
            if (document.AccessLevel == AccessLevel.Private)
                return Result<Guid>.Fail("Firstly share your document", 400);
            var existingDocumentAccess = await context.DocumentAccesses.FirstOrDefaultAsync(da => da.DocumentId == documentId && da.UserId == user.Id);
            if (existingDocumentAccess != null)
            {
                existingDocumentAccess.Role = roleEnum;
                await context.SaveChangesAsync();
                return Result<Guid>.Ok(documentId);
            }
            await context.DocumentAccesses.AddAsync(new DocumentAccess()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DocumentId = documentId,
                Role = roleEnum,
                Document = document,
                User = user
            });
            await context.SaveChangesAsync();
            return Result<Guid>.Ok(documentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> CreateDocumentAccess(Guid userId, Guid documentId)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return Result<Guid>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<Guid>.Fail("Document not found", 404);
            context.DocumentAccesses.Add(new DocumentAccess()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DocumentId = document.Id,
                Document = document,
                Role = Role.Creator,
                User = user
            });
            await context.SaveChangesAsync();
            return Result<Guid>.Ok(document.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> DeleteDocumentAccess(Guid userId, Guid documentId)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return Result<Guid>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<Guid>.Fail("Document not found", 404);
            await context.DocumentAccesses
                .Where(da => da.DocumentId == documentId && da.UserId == user.Id)
                .ExecuteDeleteAsync();
            return Result<Guid>.Ok(documentId);
        }
        catch (Exception ex)
        {
            return Result<Guid>.FromException(ex, 500);
        }
    }
    
    
}