using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("concert_setlist")]
public class ConcertSetlist
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required, MaxLength(50)]
  public string ConcertId { get; set; } = string.Empty;

  [Required, MaxLength(5)]
  public string SongId { get; set; } = string.Empty;

  [Range(1, int.MaxValue)]
  public int Position { get; set; }

  [ForeignKey(nameof(ConcertId))]
  public Concert Concert { get; set; } = null!;

  [ForeignKey(nameof(SongId))]
  public Song Song { get; set; } = null!;

  public ICollection<SetlistPerformer> Performers { get; set; } = [];
}
