using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("album_tracks")]
[PrimaryKey(nameof(AlbumId), nameof(TrackNumber))]
public class AlbumTrack
{
  [MaxLength(10)]
  public string AlbumId { get; set; } = string.Empty;

  [Range(1, int.MaxValue)]
  public int TrackNumber { get; set; }

  [Required, MaxLength(5)]
  public string SongId { get; set; } = string.Empty;

  [ForeignKey(nameof(AlbumId))]
  public Album Album { get; set; } = null!;

  [ForeignKey(nameof(SongId))]
  public Song Song { get; set; } = null!;
}
