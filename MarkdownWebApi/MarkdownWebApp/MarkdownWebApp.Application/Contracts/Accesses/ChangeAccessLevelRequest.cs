using System.ComponentModel.DataAnnotations;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Contracts.Accesses;

public record ChangeAccessLevelRequest([Required] Guid DocumentId, [Required] AccessLevelModel AccessLevel);