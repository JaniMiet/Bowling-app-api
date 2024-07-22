using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Seasons.Commands.CreateSeason;

public record DeleteSeasonCommand(string Id) : IRequest;

public class DeleteSeasonCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteSeasonCommand>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
    {
        var currentSeason = await _context.Seasons.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, currentSeason);

        _context.Seasons.Remove(currentSeason);
        await _context.SaveChangesAsync(cancellationToken);

        var seasons = await _context
            .Seasons.Include(s => s.SeasonBowlers)
            .Include(s => s.Results)
            .ToListAsync(cancellationToken);

        Season.UpdateSeasonNumbers(seasons);

        foreach (var season in seasons.OrderBy(s => s.Number))
        {
            var previousSeason = seasons.Find(s => s.Number == season.Number - 1);

            season.UpdateSeasonAndSeasonBowlerStatistics(season, previousSeason);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
