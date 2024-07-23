using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Application.Seasons.Queries.GetSeasons;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Seasons.Queries.GetSeasonReport;

public record GetMostRecentSeasonReportQuery() : IRequest<SeasonReportDto> { }

public class GetMostRecentSeasonReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMostRecentSeasonReportQuery, SeasonReportDto>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<SeasonReportDto> Handle(
        GetMostRecentSeasonReportQuery request,
        CancellationToken cancellationToken
    )
    {
        var seasonReportSeasonDto =
            await _context
                .Seasons.OrderByDescending(s => s.Number)
                .Where(s => s.Results.Count != 0)
                .Select(s => new SeasonReportSeasonDto
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
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("MostRecentSeasonQuery", nameof(Season));

        return new SeasonReportDto(seasonReportSeasonDto);
    }
}
