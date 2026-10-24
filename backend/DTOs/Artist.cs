using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public sealed record Artist(
  int Id,
  string Name,
  DateOnly Debut,
  DateOnly Birthday,
  bool IsActive);

public sealed record ArtistInput
{
  [Required, MaxLength(150)]
  public required string Name { get; init; }

  public required DateOnly Debut { get; init; }

  public required DateOnly Birthday { get; init; }

  public bool IsActive { get; init; } = true;
}
