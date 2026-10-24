namespace backend.DTOs;

// Naming:
//   X       = what the API returns
//   XInput  = request body shared by create and update
//   CreateX / UpdateX = request bodies that differ (extra id or Version)
// Times are DateTimeOffset; Npgsql only accepts UTC (offset 0) for timestamptz,
// so convert incoming values with .ToUniversalTime() before saving.

public sealed record ArtistSummary(int Id, string Name);

public sealed record StaffSummary(string Id, string Name, string JobRole);

public sealed record SongSummary(string SongId, string Title, string? ThumbnailUrl);
