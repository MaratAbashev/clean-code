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
                .Where(da => da.UserId == userId &&
                             (da.Role == Role.Creator ||
                              (da.Role != Role.Creator && da.Document!.AccessLevel != AccessLevel.Private)))
                                 .Select(da => new DocumentAccessModel
                                 {
                                     UserId = da.UserId,
                                     Role = Enum.Parse<RoleModel>(da.Role.ToString()),
                                     DocumentName = da.Document!.Name,
                                     DocumentId = da.DocumentId,
                                     Users = context.DocumentAccesses
                                         .Where(docAcc => da.DocumentId == docAcc.DocumentId)
                                         .Include(docAcc => docAcc.User)
                                         .Select(docAcc => new UserModel()
                                         {
                                             Email = docAcc.User!.Email,
                                             Id = docAcc.UserId,
                                             UserName = docAcc.User.UserName
                                         }).ToList()
                                 })
                                 .ToListAsync();
            var publicDocuments = await context.Documents
                .AsNoTracking()
                .Where(d => d.AccessLevel == AccessLevel.Public)
                .Include(d => d.UserDocuments)
                .Select(d => new DocumentAccessModel
                {
                    UserId = userId,
                    Role = RoleModel.Editor,
                    DocumentName = d.Name,
                    DocumentId = d.Id,
                    Users = d.UserDocuments!
                        .Select(docAcc => new UserModel()
                        {
                            Email = docAcc.User!.Email,
                            Id = docAcc.UserId,
                            UserName = docAcc.User.UserName
                        }).ToList()
                })
                .ToListAsync();
            documentAccessList.AddRange(publicDocuments);
            return Result<List<DocumentAccessModel>>.Ok(documentAccessList);
        }
        catch (Exception ex)
        {
            return Result<List<DocumentAccessModel>>.FromException(ex, 500);
        }
    }

    public async Task<Result<DocumentModel>> ChangeDocumentAccess(Guid userId, Guid documentId, AccessLevelModel accessLevelModel) 
    {
        try
        {
            var query = context.DocumentAccesses
                .Where(da => da.DocumentId == documentId);
            if (!await query.AnyAsync())
                return Result<DocumentModel>.Fail("Document not found", 404);
            query = query.Where(da => da.UserId == userId);
            if (!await query.AnyAsync())
                return Result<DocumentModel>.Fail("You have no access to this document", 403);
            query = query.Where(da => da.Role == Role.Creator);
            if (!await query.AnyAsync())
                return Result<DocumentModel>.Fail("Only creator can change access of this document", 403);
            var document = await query
                .Include(da => da.Document)
                .Select(da => da.Document)
                .FirstOrDefaultAsync();
            var accessLevel = Enum.Parse<AccessLevel>(accessLevelModel.ToString());
            document!.AccessLevel = accessLevel;
            await context.SaveChangesAsync();
            return Result<DocumentModel>.Ok(new DocumentModel
            {
                AccessLevel = accessLevelModel,
                DocumentId = documentId,
                DocumentName = document.Name,
            });
        }
        catch (Exception ex)
        {
            return Result<DocumentModel>.FromException(ex, 500);
        }
    }

    public async Task<Result<DocumentAccessModel>> ControlAccess(string email, Guid documentId, RoleModel roleModel) //получение доступа юзера к документу (как при переходе по ссылке так и при выдаче автором)
    {
        try
        {
            var role = Enum.Parse<Role>(roleModel.ToString());
            if (role == Role.Creator)
                return Result<DocumentAccessModel>.Fail("You can't give a creator role", 400);
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Result<DocumentAccessModel>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<DocumentAccessModel>.Fail("Document not found", 404);
            if (document.AccessLevel == AccessLevel.Private)
                return Result<DocumentAccessModel>.Fail("Document is not shared", 400);
            var existingDocumentAccess = await context.DocumentAccesses.FirstOrDefaultAsync(da => da.DocumentId == documentId && da.UserId == user.Id);
            var documentAccessModel = new DocumentAccessModel
            {
                UserId = user.Id,
                Role = roleModel,
                DocumentId = documentId,
                DocumentName = document.Name
            };
            if (existingDocumentAccess != null)
            {
                existingDocumentAccess.Role = role;
                await context.SaveChangesAsync();
                return Result<DocumentAccessModel>.Ok(documentAccessModel);
            }
            await context.DocumentAccesses.AddAsync(new DocumentAccess()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DocumentId = documentId,
                Role = role,
                Document = document,
                User = user
            });
            await context.SaveChangesAsync();
            return Result<DocumentAccessModel>.Ok(documentAccessModel);
        }
        catch (Exception ex)
        {
            return Result<DocumentAccessModel>.FromException(ex, 500);
        }
    }

    public async Task<Result<DocumentAccessModel>> JoinByLink(Guid userId, Guid documentId)
    {
        try
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return Result<DocumentAccessModel>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<DocumentAccessModel>.Fail("Document not found", 404);
            var existingDocumentAccess = await context.DocumentAccesses.FirstOrDefaultAsync(da => da.DocumentId == documentId && da.UserId == user.Id);
            var documentAccessModel = new DocumentAccessModel
            {
                UserId = user.Id,
                Role = RoleModel.Editor,
                DocumentId = documentId,
                DocumentName = document.Name
            };
            if (existingDocumentAccess != null)
            {
                documentAccessModel = new DocumentAccessModel
                {
                    UserId = user.Id,
                    Role = Enum.Parse<RoleModel>(existingDocumentAccess.Role.ToString()),
                    DocumentId = documentId,
                    DocumentName = document.Name
                };
                return Result<DocumentAccessModel>.Ok(documentAccessModel);
            }
            if (document.AccessLevel == AccessLevel.Private)
                return Result<DocumentAccessModel>.Fail("Document is not shared", 400);
            await context.DocumentAccesses.AddAsync(new DocumentAccess()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DocumentId = documentId,
                Role = Role.Editor,
                Document = document,
                User = user
            });
            await context.SaveChangesAsync();
            return Result<DocumentAccessModel>.Ok(documentAccessModel);
        }
        catch (Exception ex)
        {
            return Result<DocumentAccessModel>.FromException(ex, 500);
        }
    }

    public async Task<Result<DocumentAccessModel>> CreateDocumentAccess(Guid userId, Guid documentId) //выдача при создании, проверить может ли автор это делать
    {
        try
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                return Result<DocumentAccessModel>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<DocumentAccessModel>.Fail("Document not found", 404);
            var existingDocumentAccess = await context.DocumentAccesses.FirstOrDefaultAsync(da => da.DocumentId == documentId);
            if (existingDocumentAccess != null)
                return Result<DocumentAccessModel>.Fail("You already own your document", 409);
            var documentAccess = new DocumentAccess()
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                DocumentId = document.Id,
                Document = document,
                Role = Role.Creator,
                User = user
            };
            context.DocumentAccesses.Add(documentAccess);
            await context.SaveChangesAsync();
            var documentAccessModel = new DocumentAccessModel()
            {
                UserId = user.Id,
                DocumentId = document.Id,
                DocumentName = document.Name,
                Role = Enum.Parse<RoleModel>(documentAccess.Role.ToString()),
            };
            return Result<DocumentAccessModel>.Ok(documentAccessModel);
        }
        catch (Exception ex)
        {
            return Result<DocumentAccessModel>.FromException(ex, 500);
        }
    }

    public async Task<Result<Guid>> DeleteDocumentAccess(string email, Guid documentId) //проверить права
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return Result<Guid>.Fail("User not found", 404);
            var document = await context.Documents.FindAsync(documentId);
            if (document == null)
                return Result<Guid>.Fail("Document not found", 404);
            var documentAccess = await context.DocumentAccesses.FirstOrDefaultAsync(da => da.DocumentId == documentId);
            if (documentAccess != null && documentAccess.Role == Role.Creator)
                return Result<Guid>.Fail("You cant remove yourself from users of this document because you are creator", 400);
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

    public async Task<Result<RoleModel>> GetUserRole(Guid userId, Guid documentId)
    {
        try
        {
            var role = (await context.DocumentAccesses
                .AsNoTracking()
                .FirstOrDefaultAsync(da => da.DocumentId == documentId && da.UserId == userId))?.Role.ToString();
            if (role == null)
            {
                var documentAccess = (await context.Documents.FindAsync(documentId))?.AccessLevel;
                return documentAccess == null?
                    Result<RoleModel>.Fail("Document not found", 404):
                    documentAccess == AccessLevel.Public?
                    Result<RoleModel>.Ok(RoleModel.Editor):
                    Result<RoleModel>.Fail("You dont have access to this document", 403);
            }
            
            return Result<RoleModel>.Ok(Enum.Parse<RoleModel>(role));
        }
        catch (Exception ex)
        {
            return Result<RoleModel>.FromException(ex, 500);
        }
    }
    
}