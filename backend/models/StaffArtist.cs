using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("staff_artists")]
[PrimaryKey(nameof(StaffId), nameof(ArtistId))]
public class StaffArtist
{
  [MaxLength(10)]
  public string StaffId { get; set; } = string.Empty;

  public int ArtistId { get; set; }

  [ForeignKey(nameof(StaffId))]
  public Staff Staff { get; set; } = null!;

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;
}
