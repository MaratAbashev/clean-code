using System.ComponentModel.DataAnnotations;

namespace MarkdownWebApi.Application.Contracts.Accesses;

public record DeleteDocumentAccessRequest([Required] string Email, [Required] Guid DocumentId);