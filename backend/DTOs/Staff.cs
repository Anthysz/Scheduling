using System.ComponentModel.DataAnnotations;

namespace backend.DTOs;

public sealed record Staff(
  string Id,
  string Name,
  DateOnly JoinDate,
  DateOnly Birthday,
  string JobRole);

public record StaffInput
{
  [Required, MaxLength(100)]
  public required string Name { get; init; }

  public required DateOnly JoinDate { get; init; }

  public required DateOnly Birthday { get; init; }

  // Coordinator, Logistics, Consumption, Venue, Manager, ...
  [Required, MaxLength(30)]
  public required string JobRole { get; init; }
}

public sealed record CreateStaff : StaffInput
{
  [Required, MaxLength(10)]
  public required string Id { get; init; }
}
