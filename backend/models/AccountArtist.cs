using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("account_artists")]
public class AccountArtist
{
  [Key, MaxLength(10)]
  public string Username { get; set; } = string.Empty;

  // Must stay "Artist"; the database checks it against accounts.access_role.
  [Required, MaxLength(20)]
  public string AccessRole { get; set; } = AccessRoles.Artist;

  public int ArtistId { get; set; }

  [ForeignKey(nameof(Username))]
  public Account Account { get; set; } = null!;

  [ForeignKey(nameof(ArtistId))]
  public Artist Artist { get; set; } = null!;
}
