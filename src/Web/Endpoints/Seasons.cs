using BowlingApp.Application.Seasons.Queries.GetSeasonReport;
using BowlingApp.Application.Seasons.Queries.GetSeasons;

namespace BowlingApp.Web.Endpoints;

public class Seasons : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetSeasons).MapGet(GetSeasonReport, "{id}/Report");
    }

    public async Task<IEnumerable<SeasonDto>> GetSeasons(ISender sender)
    {
        return await sender.Send(new GetSeasonsQuery());
    }

    public async Task<SeasonReportDto> GetSeasonReport(ISender sender, string id)
    {
        return await sender.Send(new GetSeasonReportQuery(id));
    }
}
