using System.ComponentModel.DataAnnotations;
using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Seasons.Queries.GetSeasons;

public record SeasonDto
{
    [Required]
    public required string Id { get; set; }

    [Required]
    public required int Year { get; set; }

    [Required]
    public required SeasonType SeasonType { get; set; }
}
