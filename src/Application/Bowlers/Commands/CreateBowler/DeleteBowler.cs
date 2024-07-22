using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Application.Common.Services;

namespace BowlingApp.Application.Bowlers.Commands.CreateBowler;

public record DeleteBowlerCommand(string Id) : IRequest;

public class DeleteBowlerCommandHandler(IApplicationDbContext context, StatisticService statisticService)
    : IRequestHandler<DeleteBowlerCommand>
{
    private readonly IApplicationDbContext _context = context;
    private readonly StatisticService _statisticService = statisticService;

    public async Task Handle(DeleteBowlerCommand request, CancellationToken cancellationToken)
    {
        var bowler = await _context.Bowlers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        Guard.Against.NotFound(request.Id, bowler);

        _context.Bowlers.Remove(bowler);
        await _context.SaveChangesAsync(cancellationToken);

        await _statisticService.RecalculateCalculatedValues(cancellationToken);
    }
}
