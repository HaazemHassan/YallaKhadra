using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Leaderboard.Queries.Models;
using YallaKhadra.Core.Features.Leaderboard.Queries.Responses;

namespace YallaKhadra.Core.Features.Leaderboard.Queries.Handlers
{
    public class LeaderboardQueryHandler : IRequestHandler<GetLeaderboardQuery, PaginatedResult<LeaderboardEntryResponse>>
    {
        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaderboardQueryHandler(IWasteReportRepository wasteReportRepository, UserManager<ApplicationUser> userManager)
        {
            _wasteReportRepository = wasteReportRepository;
            _userManager = userManager;
        }

        public async Task<PaginatedResult<LeaderboardEntryResponse>> Handle(GetLeaderboardQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var pageNumber = Math.Max(1, request.PageNumber.GetValueOrDefault(1));
                var pageSize = Math.Max(1, request.PageSize.GetValueOrDefault(10));

                var startDate = GetStartDate(request.Period);

                var reportsQuery = _wasteReportRepository.GetTableNoTracking(r => r.Status == ReportStatus.Done);

                if (startDate.HasValue)
                {
                    reportsQuery = reportsQuery.Where(r => r.CreatedAt >= startDate.Value);
                }

                var leaderboardQuery = from report in reportsQuery
                                       group report by report.UserId into g
                                       select new
                                       {
                                           UserId = g.Key,
                                           TotalReportsCount = g.Count()
                                       };

                var rankedQuery = from entry in leaderboardQuery
                                  join user in _userManager.Users.Include(u => u.ProfileImage)
                                  on entry.UserId equals user.Id
                                  orderby entry.TotalReportsCount descending
                                  select new
                                  {
                                      entry.UserId,
                                      FullName = user.FirstName + " " + user.LastName,
                                      ProfileImageUrl = user.ProfileImage != null ? user.ProfileImage.Url : null,
                                      entry.TotalReportsCount
                                  };

                var totalCount = await rankedQuery.CountAsync(cancellationToken);

                if (totalCount == 0)
                {
                    return PaginatedResult<LeaderboardEntryResponse>.Success([], totalCount, pageNumber, pageSize);
                }

                var pagedItems = await rankedQuery
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken);

                var rankOffset = (pageNumber - 1) * pageSize;

                var result = pagedItems.Select((item, index) => new LeaderboardEntryResponse
                {
                    UserId = item.UserId,
                    FullName = item.FullName,
                    ProfileImageUrl = item.ProfileImageUrl,
                    TotalReportsCount = item.TotalReportsCount,
                    Rank = rankOffset + index + 1
                }).ToList();

                return PaginatedResult<LeaderboardEntryResponse>.Success(result, totalCount, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return PaginatedResult<LeaderboardEntryResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        private static DateTime? GetStartDate(LeaderboardPeriod period)
        {
            return period switch
            {
                LeaderboardPeriod.Weekly => DateTime.UtcNow.AddDays(-7),
                LeaderboardPeriod.Monthly => DateTime.UtcNow.AddMonths(-1),
                LeaderboardPeriod.Yearly => DateTime.UtcNow.AddYears(-1),
                _ => null
            };
        }
    }
}
