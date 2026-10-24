using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public sealed record ConcertSummary(
  string ConcertId,
  string ConcertName,
  DateTimeOffset EstimatedStart,
  DateTimeOffset EstimatedEnd,
  string VenueLocation);

public sealed record SetlistEntry(
  int Id,
  int Position,
  SongSummary Song,
  IReadOnlyList<ArtistSummary> Performers);

public sealed record Concert(
  string ConcertId,
  string ConcertName,
  DateTimeOffset EstimatedStart,
  DateTimeOffset EstimatedEnd,
  string VenueLocation,
  IReadOnlyList<ArtistSummary> Artists,
  IReadOnlyList<SetlistEntry> Setlist,
  DateTimeOffset CreatedAt,
  DateTimeOffset UpdatedAt,
  string? CreatedBy,
  string? UpdatedBy,
  uint Version);

public record ConcertInput : IValidatableObject
{
  [Required, MaxLength(200)]
  public required string ConcertName { get; init; }

  public required DateTimeOffset EstimatedStart { get; init; }

  public required DateTimeOffset EstimatedEnd { get; init; }

  [Required, MaxLength(200)]
  public required string VenueLocation { get; init; }

  public IReadOnlyList<int> ArtistIds { get; init; } = [];

  public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
  {
    if (EstimatedStart >= EstimatedEnd)
    {
      yield return new ValidationResult(
        "EstimatedEnd must be after EstimatedStart.",
        [nameof(EstimatedEnd)]);
    }
  }
}

public sealed record CreateConcert : ConcertInput
{
  // Example: ISEKAIJOUCHO-ANIMAIV
  [Required, MaxLength(50)]
  public required string ConcertId { get; init; }
}

public sealed record UpdateConcert : ConcertInput
{
  // Send back the Version you received; a mismatch means someone else edited it.
  public required uint Version { get; init; }
}

public sealed record SetlistEntryInput
{
  [Required, MaxLength(5)]
  public required string SongId { get; init; }

  [Range(1, int.MaxValue)]
  public required int Position { get; init; }

  public IReadOnlyList<int> PerformerIds { get; init; } = [];
}

// Replaces the whole setlist so reordering happens in one transaction.
public sealed record ReplaceSetlist
{
  public required IReadOnlyList<SetlistEntryInput> Entries { get; init; }
}
