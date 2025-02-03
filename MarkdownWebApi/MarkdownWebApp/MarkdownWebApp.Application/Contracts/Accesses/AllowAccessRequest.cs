using System.ComponentModel.DataAnnotations;
using MarkdownWebApi.Core.Models;

namespace MarkdownWebApi.Application.Contracts.Accesses;

public record AllowAccessRequest([Required] string Email, [Required] Guid DocumentId, [Required] RoleModel Role);