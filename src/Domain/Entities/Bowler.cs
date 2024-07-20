namespace BowlingApp.Domain.Entities;

public class Bowler : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Gender Gender { get; set; }
    public List<SeasonBowler> SeasonBowlers { get; set; } = [];
}
