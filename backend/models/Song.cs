using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("songs")]
public class Song
{
  // Example: x9aS8
  [Key, MaxLength(5)]
  public string SongId { get; set; } = string.Empty;

  [MaxLength(15)]
  public string? YoutubeId { get; set; }

  [Required, MaxLength(100)]
  public string Title { get; set; } = string.Empty;

  // Exclusive tracks, unreleased tracks, etc.
  [MaxLength(1000)]
  public string? SpecialNotes { get; set; }

  public DateTimeOffset? ReleaseDate { get; set; }

  [MaxLength(500)]
  public string? ThumbnailUrl { get; set; }

  public ICollection<SongArtist> Artists { get; set; } = [];

  public ICollection<SongCredit> Credits { get; set; } = [];
}
