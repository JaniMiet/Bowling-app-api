using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Application.Common.Services;

namespace BowlingApp.Application.Seasons.Commands.CreateSeason;

public record DeleteSeasonCommand(string Id) : IRequest;

public class DeleteSeasonCommandHandler(IApplicationDbContext context, StatisticService statisticService)
    : IRequestHandler<DeleteSeasonCommand>
{
    private readonly IApplicationDbContext _context = context;
    private readonly StatisticService _statisticService = statisticService;

    public async Task Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
    {
        var currentSeason = await _context.Seasons.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, currentSeason);

        _context.Seasons.Remove(currentSeason);
        await _context.SaveChangesAsync(cancellationToken);

        await _statisticService.RecalculateCalculatedValues(cancellationToken);
    }
}
