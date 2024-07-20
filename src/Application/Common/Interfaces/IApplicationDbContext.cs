using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Bowler> Bowlers { get; }
    DbSet<Result> Results { get; }
    DbSet<Season> Seasons { get; }
    DbSet<SeasonBowler> SeasonBowlers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
