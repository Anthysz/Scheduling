using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace backend.Models;

[Table("schedule_staff")]
[PrimaryKey(nameof(ScheduleId), nameof(StaffId))]
public class ScheduleStaff
{
  [MaxLength(5)]
  public string ScheduleId { get; set; } = string.Empty;

  [MaxLength(10)]
  public string StaffId { get; set; } = string.Empty;

  [ForeignKey(nameof(ScheduleId))]
  public ScheduleItem Schedule { get; set; } = null!;

  [ForeignKey(nameof(StaffId))]
  public Staff Staff { get; set; } = null!;
}
