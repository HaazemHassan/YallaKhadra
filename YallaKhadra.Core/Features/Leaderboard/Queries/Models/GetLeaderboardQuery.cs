using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Leaderboard.Queries.Responses;

namespace YallaKhadra.Core.Features.Leaderboard.Queries.Models
{
    public class GetLeaderboardQuery : IRequest<PaginatedResult<LeaderboardEntryResponse>>
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public LeaderboardPeriod Period { get; set; } = LeaderboardPeriod.AllTime;
    }
}
