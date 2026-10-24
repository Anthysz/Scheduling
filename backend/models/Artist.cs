using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("artists")]
public class Artist
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  // Stage name, not real name
  [Required, MaxLength(150)]
  public string Name { get; set; } = string.Empty;

  public DateOnly Debut { get; set; }

  public DateOnly Birthday { get; set; }

  public bool IsActive { get; set; } = true;
}
