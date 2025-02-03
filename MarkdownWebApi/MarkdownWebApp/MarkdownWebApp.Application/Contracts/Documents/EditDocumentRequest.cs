using System.ComponentModel.DataAnnotations;

namespace MarkdownWebApi.Application.Contracts.Documents;

public record EditDocumentRequest([Required] string Content);