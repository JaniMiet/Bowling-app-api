using System.Reflection;
using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BowlingApp.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<TodoList> TodoLists => Set<TodoList>();
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    public DbSet<Bowler> Bowlers => Set<Bowler>();
    public DbSet<Result> Results => Set<Result>();
    public DbSet<Season> Seasons => Set<Season>();
    public DbSet<SeasonBowler> SeasonBowlers => Set<SeasonBowler>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
