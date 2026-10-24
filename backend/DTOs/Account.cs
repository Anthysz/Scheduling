using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs;

public sealed record Account(
  string Username,
  string AccessRole,
  DateTimeOffset LastLogin,
  DateTimeOffset CreatedAt,
  int? ArtistId,
  string? StaffId,
  string hashedPassword
);

public sealed record UpdateAccountRole
{
  [Required]
  [AllowedValues(
    AccessRoles.Viewer,
    AccessRoles.Admin,
    AccessRoles.Artist,
    AccessRoles.Staff)]
  public required string AccessRole { get; init; }
  
}
