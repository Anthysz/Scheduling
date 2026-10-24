using System.ComponentModel.DataAnnotations;
using backend.Models;

namespace backend.DTOs;

public sealed record AlbumTrack(int TrackNumber, SongSummary Song);

public sealed record Album(
  string AlbumId,
  string Title,
  DateOnly ReleaseDate,
  string CoverUrl,
  string AlbumType,
  IReadOnlyList<AlbumTrack> Tracks);

public sealed record AlbumTrackInput
{
  [Required, MaxLength(5)]
  public required string SongId { get; init; }

  [Range(1, int.MaxValue)]
  public required int TrackNumber { get; init; }
}

// On update, Tracks replaces the whole track list (reordering in one transaction).
public record AlbumInput
{
  [Required, MaxLength(150)]
  public required string Title { get; init; }

  public required DateOnly ReleaseDate { get; init; }

  [Required, MaxLength(500)]
  public required string CoverUrl { get; init; }

  [Required]
  [AllowedValues(
    AlbumTypes.Single,
    AlbumTypes.EP,
    AlbumTypes.Album,
    AlbumTypes.Compilation)]
  public required string AlbumType { get; init; }

  public IReadOnlyList<AlbumTrackInput> Tracks { get; init; } = [];
}

public sealed record CreateAlbum : AlbumInput
{
  [Required, MaxLength(10)]
  public required string AlbumId { get; init; }
}
