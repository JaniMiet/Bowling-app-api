using System.Globalization;
using System.Text.RegularExpressions;
using BowlingApp.Application.Common.Services;
using BowlingApp.Application.Seasons.Queries.GetSeasons;
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
    ApplicationDbContext context,
    StatisticService statisticService
)
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
    private readonly ApplicationDbContext _context = context;
    private readonly StatisticService _statisticService = statisticService;

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
            await TrySeedDataFromCsvsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedDataFromCsvsAsync()
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

        var csvResults = new List<CsvResult>();

        var seasonNumber = 0;

        List<string> bowlerNames = [];
        List<SeasonDto> seasonDtos = [];

        foreach (var file in files)
        {
            seasonNumber++;
            // Get the file name without the path
            string fileName = file.fileName;
            var year = file.year;
            var seasonType = file.seasonTypeNumber == 1 ? SeasonType.Spring : SeasonType.Autumn;

            var season = new SeasonDto()
            {
                SeasonType = seasonType,
                Year = year,
                Id = Guid.NewGuid().ToString(),
            };

            seasonDtos.Add(season);

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
                        bowlerNames.Add(name);
                    }
                    else
                    {
                        MatchCollection matches = Regex.Matches(player.Key, pattern);
                        var isValidScore = int.TryParse(player.Value, out int score);

                        if (isValidScore && score != 0)
                        {
                            csvResults.Add(
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

        /// Adding Bowlers
        bowlerNames = bowlerNames.Distinct().ToList();
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
        }
        await _context.SaveChangesAsync();

        var bowlers = await _context.Bowlers.ToListAsync();

        // Adding Seasons / SeasonBowlers
        var seasons = new List<Season>();

        foreach (var season in seasonDtos)
        {
            seasons.Add(new Season(season.Year, season.SeasonType, 0, bowlers));
        }

        _context.AddRange(seasons);
        Season.UpdateSeasonNumbers(seasons);
        await _context.SaveChangesAsync();

        // Adding Results
        foreach (var csvResult in csvResults)
        {
            var nameParts = csvResult.Name.Split(' ');
            var firstName = nameParts[0];
            var lastName = nameParts[1];

            var seasonBowlers = seasons.SelectMany(s => s.SeasonBowlers);

            var seasonBowlerId = seasonBowlers
                .Single(sb =>
                    sb.Bowler.FirstName == firstName
                    && sb.Bowler.LastName == lastName
                    && sb.Season.Year == csvResult.Year
                    && sb.Season.SeasonType == csvResult.SeasonType
                )
                .Id;

            var seasonId = seasons.Single(s => s.Year == csvResult.Year && s.SeasonType == csvResult.SeasonType).Id;

            _context.Results.Add(new Result(csvResult.Score, csvResult.WeekNumber, seasonBowlerId, seasonId));
        }

        await _context.SaveChangesAsync();
        await _statisticService.RecalculateCalculatedValues(new CancellationToken());
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
