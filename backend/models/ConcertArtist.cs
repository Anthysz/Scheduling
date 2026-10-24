using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("concert_artists")]
[PrimaryKey(nameof(ConcertId), nameof(ArtistId))]
public class ConcertArtist
{
  [MaxLength(50)]
  public string ConcertId { get; set; } = string.Empty;

  public int ArtistId { get; set; }

  [ForeignKey(nameof(ConcertId))]
  public Concert Concert { get; set; } = null!;

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;
}
