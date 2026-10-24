using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("setlist_performers")]
[PrimaryKey(nameof(SetlistId), nameof(ArtistId))]
public class SetlistPerformer
{
  public int SetlistId { get; set; }

  // Must match the setlist entry's concert; the database enforces it.
  [Required, MaxLength(50)]
  public string ConcertId { get; set; } = string.Empty;

  public int ArtistId { get; set; }

  [ForeignKey(nameof(SetlistId))]
  public ConcertSetlist Setlist { get; set; } = null!;

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;
}
