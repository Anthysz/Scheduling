using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public sealed record Contributor(int Id, string Name, string? Contact);

public sealed record ContributorInput
{
  [Required, MaxLength(100)]
  public required string Name { get; init; }

  [MaxLength(200)]
  public string? Contact { get; init; }
}
