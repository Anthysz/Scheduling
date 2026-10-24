using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("song_credits")]
[PrimaryKey(nameof(SongId), nameof(ContributorId), nameof(Role))]
public class SongCredit
{
  [MaxLength(5)]
  public string SongId { get; set; } = string.Empty;

  public int ContributorId { get; set; }

  [MaxLength(100)]
  public string Role { get; set; } = string.Empty;

  [ForeignKey(nameof(SongId))]
  public Song Song { get; set; } = null!;

  [ForeignKey(nameof(ContributorId))]
  public Contributor Contributor { get; set; } = null!;
}
