using System.Globalization;
using System.Text.RegularExpressions;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Enums;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BowlingApp.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
        await initialiser.InitialiseAsync(resetDatabase: false);
    }
}

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context
)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly ApplicationDbContext _context = context;

    public async Task InitialiseAsync(bool resetDatabase)
    {
        try
        {
            if (resetDatabase)
            {
                await _context.Database.EnsureDeletedAsync();
                await _context.Database.EnsureCreatedAsync();
                await SeedAsync();
            }
            else
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        // Get all CSV files in the current directory
        string[] csvFiles = Directory.GetFiles(currentDirectory, "*.csv");

        var files = csvFiles
            .Select(file => new
            {
                fileName = Path.GetFileName(file),
                year = int.Parse(Path.GetFileName(file).Split('-')[0]),
                seasonTypeNumber = int.Parse(Path.GetFileName(file).Split('-')[1].Split('.')[0])
            })
            .OrderBy(x => x.year)
            .ThenBy(x => x.seasonTypeNumber);

        var results = new List<CsvResult>();

        var seasonNumber = 0;

        foreach (var file in files)
        {
            seasonNumber++;
            // Get the file name without the path
            string fileName = file.fileName;
            var year = file.year;
            var seasonType = file.seasonTypeNumber == 1 ? SeasonType.Spring : SeasonType.Autumn;

            var season = new Season()
            {
                SeasonType = seasonType,
                Year = year,
                Number = seasonNumber
            };
            await _context.Seasons.AddAsync(season);
            await _context.SaveChangesAsync();

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(fileName, System.Text.Encoding.Latin1);
            using var csv = new CsvReader(reader, csvConfig);
            var records = csv.GetRecords<dynamic>();

            // Define a regex pattern to match numbers
            string pattern = @"\d+";

            foreach (var record in records)
            {
                string name = "";
                var index = 0;

                foreach (var player in record)
                {
                    if (index == 0)
                    {
                        name = player.Value;
                    }
                    else
                    {
                        MatchCollection matches = Regex.Matches(player.Key, pattern);
                        var isValidScore = int.TryParse(player.Value, out int score);

                        if (isValidScore && score != 0)
                        {
                            results.Add(
                                new CsvResult
                                {
                                    WeekNumber = int.Parse(matches[0].Value),
                                    Score = score,
                                    Year = year,
                                    Name = name,
                                    SeasonType = seasonType,
                                }
                            );
                        }
                    }
                    index++;
                }
            }
        }
        await _context.SaveChangesAsync();
        var bowlerNames = results.Select(r => r.Name).Distinct();
        foreach (var bowlerName in bowlerNames)
        {
            var nameParts = bowlerName.Split(' ');
            var bowler = new Bowler
            {
                FirstName = nameParts[0],
                LastName = nameParts[1],
                Gender = Gender.Male,
            };

            await _context.Bowlers.AddAsync(bowler);
            await _context.SaveChangesAsync();
        }

        var seasons = await _context.Seasons.ToListAsync();
        foreach (var season in seasons)
        {
            var bowlers = await _context.Bowlers.ToListAsync();
            foreach (var bowler in bowlers)
            {
                var seasonBowler = new SeasonBowler { Season = season, Bowler = bowler };
                await _context.SeasonBowlers.AddAsync(seasonBowler);
                await _context.SaveChangesAsync();

                var bowlerSeasonResults = results
                    .Where(x =>
                        x.Name.Contains(bowler.FirstName)
                        && x.Name.Contains(bowler.LastName)
                        && x.SeasonType == season.SeasonType
                        && x.Year == season.Year
                    )
                    .ToList();

                foreach (var result in bowlerSeasonResults)
                {
                    await _context.Results.AddAsync(
                        new Result
                        {
                            Score = result.Score,
                            Year = result.Year,
                            Week = result.WeekNumber,
                            SeasonBowler = seasonBowler
                        }
                    );
                }

                await _context.SaveChangesAsync();
            }
        }

        foreach (var season in seasons)
        {
            var currentSeasonResults = await _context
                .Results.Where(r => r.SeasonBowler.SeasonId == season.Id)
                .ToListAsync();
            var previousSeasonResults = await _context
                .Results.Where(r => r.SeasonBowler.Season.Number == season.Number - 1)
                .ToListAsync();

            season.UpdateSeasonStatistics(currentSeasonResults, previousSeasonResults);
        }

        await _context.SaveChangesAsync();

        var allResults = await _context
            .Results.Include(r => r.SeasonBowler)
            .ThenInclude(sb => sb.Season)
            .Include(r => r.SeasonBowler)
            .ThenInclude(r => r.Bowler)
            .ToListAsync();
        var seasonBowlers = allResults.Select(r => r.SeasonBowler).Distinct();

        foreach (var seasonBowler in seasonBowlers)
        {
            var currentSeasonNumber = seasonBowler.Season.Number;
            var previousSeasonNumber = currentSeasonNumber - 1;
            var currentSeasonResults = allResults
                .Where(r =>
                    r.SeasonBowler.Season.Number == currentSeasonNumber
                    && r.SeasonBowler.BowlerId == seasonBowler.BowlerId
                )
                .ToList();
            var previousSeasonResults = allResults
                .Where(r =>
                    r.SeasonBowler.Season.Number == previousSeasonNumber
                    && r.SeasonBowler.BowlerId == seasonBowler.BowlerId
                )
                .ToList();

            seasonBowler.UpdateSeasonBowlerStatistics(currentSeasonResults, previousSeasonResults);
        }

        await _context.SaveChangesAsync();
    }
}

public class CsvResult
{
    public required int WeekNumber { get; set; }
    public required int Score { get; set; }
    public required string Name { get; set; }
    public required int Year { get; set; }
    public required SeasonType SeasonType { get; set; }
}
