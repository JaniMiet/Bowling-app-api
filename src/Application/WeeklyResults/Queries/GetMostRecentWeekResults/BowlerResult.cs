namespace BowlingApp.Application.WeeklyResults.Queries.GetMostRecentWeekResults;

public record BowlerResult
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required int Score { get; init; }
    public required double SetAverageScore { get; init; }
}
