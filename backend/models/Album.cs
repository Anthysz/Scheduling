using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("albums")]
public class Album
{
  [Key, MaxLength(10)]
  public string AlbumId { get; set; } = string.Empty;

  [Required, MaxLength(150)]
  public string Title { get; set; } = string.Empty;

  public DateOnly ReleaseDate { get; set; }

  [Required, MaxLength(500)]
  public string CoverUrl { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  public string AlbumType { get; set; } = AlbumTypes.Single;

  public ICollection<AlbumTrack> Tracks { get; set; } = [];
}
