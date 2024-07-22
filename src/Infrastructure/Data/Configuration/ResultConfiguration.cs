using BowlingApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BowlingApp.Infrastructure.Data.Configuration;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> builder)
    {
        builder.HasIndex(p => new { p.Week, p.SeasonBowlerId }).IsUnique();
    }
}
