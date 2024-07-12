using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Application.Seasons.Queries.GetSeasons;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Seasons.Queries.GetSeasonReport;

public record GetSeasonReportQuery(string Id) : IRequest<SeasonReportDto> { }

public class GetSeasonReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSeasonReportQuery, SeasonReportDto>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<SeasonReportDto> Handle(GetSeasonReportQuery request, CancellationToken cancellationToken)
    {
        var seasonReportSeasonDto =
            await _context
                .Seasons.Select(s => new SeasonReportSeasonDto
                {
                    Id = s.Id,
                    Year = s.Year,
                    SeasonType = s.SeasonType,
                    Bowlers = s
                        .SeasonBowlers.Select(sb => new BowlerDto
                        {
                            Id = sb.BowlerId,
                            FirstName = sb.Bowler.FirstName,
                            LastName = sb.Bowler.LastName,
                            Gender = sb.Bowler.Gender,
                            Results = sb.Results.Select(r => new ResultDto { Score = r.Score, Week = r.Week }).ToList(),
                            Average = sb.SeasonAverage,
                            ChangeToPreviousSeason = sb.ChangeToPreviousSeason,
                        })
                        .ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(request.Id, nameof(Season));

        return new SeasonReportDto(seasonReportSeasonDto);
    }
}

public class SeasonReportSeasonDto
{
    public required string Id { get; set; }
    public required int Year { get; set; }
    public required SeasonType SeasonType { get; set; }
    public List<BowlerDto> Bowlers { get; set; } = [];
}

public class BowlerDto
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Gender Gender { get; set; }
    public required double? Average { get; set; }
    public required double? ChangeToPreviousSeason { get; set; }
    public List<ResultDto> Results { get; set; } = [];
}

public class ResultDto
{
    public required int Score { get; set; }
    public required int Week { get; set; }
}
