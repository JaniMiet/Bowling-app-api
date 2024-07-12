using System.ComponentModel.DataAnnotations;

namespace BowlingApp.Application.WeeklyResults.Queries.GetWeeklyResultsByBowlerId;

public record class ResultDto
{
    [Required]
    public required string Id { get; set; }

    [Required]
    public int Score { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public int Week { get; set; }
}
