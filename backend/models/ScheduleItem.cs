using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("schedule")]
public class ScheduleItem
{
  [Key, MaxLength(5)]
  public string Id { get; set; } = string.Empty;

  public int ArtistId { get; set; }

  [Required, MaxLength(100)]
  public string ActivityTitle { get; set; } = string.Empty;

  [MaxLength(2500)]
  public string? ActivityDescription { get; set; }

  public DateTimeOffset StartTime { get; set; }

  public DateTimeOffset EstimatedEndTime { get; set; }

  // Venue, office, etc.
  [Required, MaxLength(200)]
  public string Location { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  public string Category { get; set; } = ScheduleCategories.Other;

  [Required, MaxLength(20)]
  public string Status { get; set; } = ScheduleStatuses.Planned;

  // Visible to Viewer accounts?
  public bool IsPublic { get; set; }

  // Null if unrelated to a concert
  [MaxLength(50)]
  public string? ConcertId { get; set; }

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

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;

  [ForeignKey(nameof(ConcertId))]
  public Concert? Concert { get; set; }

  public ICollection<ScheduleStaff> StaffAssignments { get; set; } = [];
}
