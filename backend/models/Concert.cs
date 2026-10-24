using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("concerts")]
public class Concert
{
  // Example: ISEKAIJOUCHO-ANIMAIV
  [Key, MaxLength(50)]
  public string ConcertId { get; set; } = string.Empty;

  [Required, MaxLength(200)]
  public string ConcertName { get; set; } = string.Empty;

  public DateTimeOffset EstimatedStart { get; set; }

  public DateTimeOffset EstimatedEnd { get; set; }

  [Required, MaxLength(200)]
  public string VenueLocation { get; set; } = string.Empty;

  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

  [MaxLength(10)]
  public string? CreatedBy { get; set; }

  [MaxLength(10)]
  public string? UpdatedBy { get; set; }

  // PostgreSQL system column used as the optimistic concurrency token.
  [Column("xmin", TypeName = "xid")]
  [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
  [ConcurrencyCheck]
  public uint Version { get; set; }

  public ICollection<ConcertArtist> Artists { get; set; } = [];

  public ICollection<ConcertSetlist> Setlist { get; set; } = [];

  public ICollection<ScheduleItem> ScheduleItems { get; set; } = [];
}
