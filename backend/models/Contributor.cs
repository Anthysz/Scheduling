using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("contributors")]
public class Contributor
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required, MaxLength(100)]
  public string Name { get; set; } = string.Empty;

  [MaxLength(200)]
  public string? Contact { get; set; }

  public ICollection<SongCredit> Credits { get; set; } = [];
}
