using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Dashboard.Queries.Responses;

namespace YallaKhadra.Core.Features.Dashboard.Queries.Models
{
    public class GetDashboardTrendsQuery : IRequest<Response<DashboardTrendsResponse>>
    {
        public LeaderboardPeriod Period { get; set; } = LeaderboardPeriod.AllTime;
    }
}
