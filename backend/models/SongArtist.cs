using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("song_artists")]
[PrimaryKey(nameof(SongId), nameof(ArtistId))]
public class SongArtist
{
  [MaxLength(5)]
  public string SongId { get; set; } = string.Empty;

  public int ArtistId { get; set; }

  [ForeignKey(nameof(SongId))]
  public Song Song { get; set; } = null!;

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;
}
