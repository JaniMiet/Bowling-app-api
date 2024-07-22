using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Common.Services;

public class StatisticService(IApplicationDbContext context)
{
    private readonly IApplicationDbContext _context = context;

    public async Task RecalculateCalculatedValues(CancellationToken cancellationToken)
    {
        var seasons = await _context
            .Seasons.Include(s => s.Results)
            .Include(s => s.SeasonBowlers)
            .ToListAsync(cancellationToken);

        Season.UpdateSeasonNumbers(seasons);

        foreach (var season in seasons)
        {
            var previousSeason = seasons.Find(s => s.Number == season.Number - 1);
            season.UpdateSeasonAndSeasonBowlerStatistics(season, previousSeason);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
