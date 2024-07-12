using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Bowlers.Queries.GetBowlers;

public record BowlerDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Gender Gender { get; set; }
}
