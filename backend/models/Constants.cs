namespace backend.Models;

// Values mirror the CHECK constraints in schema.sql.

public static class AccessRoles
{
  public const string Viewer = "Viewer";
  public const string Admin = "Admin";
  public const string Artist = "Artist";
  public const string Staff = "Staff";
}

public static class AlbumTypes
{
  public const string Single = "Single";
  public const string EP = "EP";
  public const string Album = "Album";
  public const string Compilation = "Compilation";
}

public static class ScheduleCategories
{
  public const string Concert = "Concert";
  public const string Recording = "Recording";
  public const string Rehearsal = "Rehearsal";
  public const string Interview = "Interview";
  public const string Event = "Event";
  public const string Off = "Off";
  public const string Other = "Other";
}

public static class ScheduleStatuses
{
  public const string Planned = "Planned";
  public const string Confirmed = "Confirmed";
  public const string Cancelled = "Cancelled";
}
