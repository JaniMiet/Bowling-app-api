using BowlingApp.Application.Seasons.Commands.CreateSeason;
using BowlingApp.Application.Seasons.Queries.GetSeasonReport;
using BowlingApp.Application.Seasons.Queries.GetSeasons;

namespace BowlingApp.Web.Endpoints;

public class Seasons : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetSeasons).MapGet(GetSeasonReport, "{id}/Report");
        app.MapGroup(this).MapPost(CreateSeason);
    }

    public async Task<IEnumerable<SeasonDto>> GetSeasons(ISender sender)
    {
        return await sender.Send(new GetSeasonsQuery());
    }

    public async Task<string> CreateSeason(ISender sender, CreateSeasonCommand createSeasonCommand)
    {
        return await sender.Send(createSeasonCommand);
    }

    public async Task<SeasonReportDto> GetSeasonReport(ISender sender, string id)
    {
        return await sender.Send(new GetSeasonReportQuery(id));
    }
}
