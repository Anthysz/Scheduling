using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend;

// The database is created from schema.sql, so this context only describes
// how entities relate and how EF should behave on delete. Keys, lengths and
// table names come from the DataAnnotations on the models; column names come
// from UseSnakeCaseNamingConvention() on the UseNpgsql(...) call.
// Delete behaviors mirror the ON DELETE rules in schema.sql.
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
  public DbSet<Contributor> Contributors => Set<Contributor>();
  public DbSet<Account> Accounts => Set<Account>();
  public DbSet<Staff> Staff => Set<Staff>();
  public DbSet<Artist> Artists => Set<Artist>();
  public DbSet<AccountArtist> AccountArtists => Set<AccountArtist>();
  public DbSet<AccountStaff> AccountStaff => Set<AccountStaff>();
  public DbSet<StaffArtist> StaffArtists => Set<StaffArtist>();
  public DbSet<Album> Albums => Set<Album>();
  public DbSet<Song> Songs => Set<Song>();
  public DbSet<SongArtist> SongArtists => Set<SongArtist>();
  public DbSet<AlbumTrack> AlbumTracks => Set<AlbumTrack>();
  public DbSet<SongCredit> SongCredits => Set<SongCredit>();
  public DbSet<Concert> Concerts => Set<Concert>();
  public DbSet<ConcertArtist> ConcertArtists => Set<ConcertArtist>();
  public DbSet<ConcertSetlist> ConcertSetlists => Set<ConcertSetlist>();
  public DbSet<SetlistPerformer> SetlistPerformers => Set<SetlistPerformer>();
  public DbSet<ScheduleItem> ScheduleItems => Set<ScheduleItem>();
  public DbSet<ScheduleStaff> ScheduleStaff => Set<ScheduleStaff>();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);

    // ---------------------------------------------------------------
    // People and access
    // ---------------------------------------------------------------

    // One account links to at most one artist and one staff member
    // (the link columns are UNIQUE in the database).
    modelBuilder.Entity<AccountArtist>(e =>
    {
      e.HasOne(x => x.Account)
        .WithOne()
        .HasForeignKey<AccountArtist>(x => x.Username)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Artist)
        .WithOne()
        .HasForeignKey<AccountArtist>(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<AccountStaff>(e =>
    {
      e.HasOne(x => x.Account)
        .WithOne()
        .HasForeignKey<AccountStaff>(x => x.Username)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Staff)
        .WithOne()
        .HasForeignKey<AccountStaff>(x => x.StaffId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<StaffArtist>(e =>
    {
      e.HasOne(x => x.Staff)
        .WithMany()
        .HasForeignKey(x => x.StaffId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Artist)
        .WithMany()
        .HasForeignKey(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    // ---------------------------------------------------------------
    // Music catalogue
    // ---------------------------------------------------------------

    modelBuilder.Entity<SongArtist>(e =>
    {
      e.HasOne(x => x.Song)
        .WithMany(x => x.Artists)
        .HasForeignKey(x => x.SongId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Artist)
        .WithMany()
        .HasForeignKey(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<AlbumTrack>(e =>
    {
      e.HasOne(x => x.Album)
        .WithMany(x => x.Tracks)
        .HasForeignKey(x => x.AlbumId)
        .OnDelete(DeleteBehavior.Cascade);

      // A song on an album cannot be deleted.
      e.HasOne(x => x.Song)
        .WithMany()
        .HasForeignKey(x => x.SongId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    modelBuilder.Entity<SongCredit>(e =>
    {
      e.HasOne(x => x.Song)
        .WithMany(x => x.Credits)
        .HasForeignKey(x => x.SongId)
        .OnDelete(DeleteBehavior.Cascade);

      // A contributor with credits cannot be deleted.
      e.HasOne(x => x.Contributor)
        .WithMany(x => x.Credits)
        .HasForeignKey(x => x.ContributorId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    // ---------------------------------------------------------------
    // Concerts
    // ---------------------------------------------------------------

    modelBuilder.Entity<ConcertArtist>(e =>
    {
      e.HasOne(x => x.Concert)
        .WithMany(x => x.Artists)
        .HasForeignKey(x => x.ConcertId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Artist)
        .WithMany()
        .HasForeignKey(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<ConcertSetlist>(e =>
    {
      e.HasOne(x => x.Concert)
        .WithMany(x => x.Setlist)
        .HasForeignKey(x => x.ConcertId)
        .OnDelete(DeleteBehavior.Cascade);

      // A song used in a setlist cannot be deleted.
      e.HasOne(x => x.Song)
        .WithMany()
        .HasForeignKey(x => x.SongId)
        .OnDelete(DeleteBehavior.Restrict);
    });

    // The database also checks that (concert_id, artist_id) is a row in
    // concert_artists, so a performer must be an artist of that concert.
    modelBuilder.Entity<SetlistPerformer>(e =>
    {
      e.HasOne(x => x.Setlist)
        .WithMany(x => x.Performers)
        .HasForeignKey(x => x.SetlistId)
        .OnDelete(DeleteBehavior.Cascade);

      e.HasOne(x => x.Artist)
        .WithMany()
        .HasForeignKey(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Cascade);
    });

    // ---------------------------------------------------------------
    // Schedule
    // ---------------------------------------------------------------

    // The database also checks that (concert_id, artist_id) is a row in
    // concert_artists, and sets concert_id to NULL when that link goes away.
    modelBuilder.Entity<ScheduleItem>(e =>
    {
      // An artist with schedule items cannot be deleted.
      e.HasOne(x => x.Artist)
        .WithMany()
        .HasForeignKey(x => x.ArtistId)
        .OnDelete(DeleteBehavior.Restrict);

      // Deleting a concert keeps its activities and clears ConcertId.
      e.HasOne(x => x.Concert)
        .WithMany(x => x.ScheduleItems)
        .HasForeignKey(x => x.ConcertId)
        .OnDelete(DeleteBehavior.SetNull);
    });

    modelBuilder.Entity<ScheduleStaff>(e =>
    {
      e.HasOne(x => x.Schedule)
        .WithMany(x => x.StaffAssignments)
        .HasForeignKey(x => x.ScheduleId)
        .OnDelete(DeleteBehavior.Cascade);

      // Staff assigned to activities cannot be deleted.
      e.HasOne(x => x.Staff)
        .WithMany()
        .HasForeignKey(x => x.StaffId)
        .OnDelete(DeleteBehavior.Restrict);
    });
  }
}
