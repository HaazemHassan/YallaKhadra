using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Dashboard.Queries.Models;
using YallaKhadra.Core.Features.Dashboard.Queries.Responses;

namespace YallaKhadra.Core.Features.Dashboard.Queries.Handlers
{
    public class DashboardQueryHandler : ResponseHandler,
        IRequestHandler<GetDashboardAnalyticsQuery, Response<DashboardAnalyticsResponse>>,
        IRequestHandler<GetDashboardTrendsQuery, Response<DashboardTrendsResponse>>
    {
        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly ICleanupTaskRepository _cleanupTaskRepository;
        private readonly IAIWasteScanRepository _aiWasteScanRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardQueryHandler(
            IWasteReportRepository wasteReportRepository,
            ICleanupTaskRepository cleanupTaskRepository,
            IAIWasteScanRepository aiWasteScanRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            UserManager<ApplicationUser> userManager)
        {
            _wasteReportRepository = wasteReportRepository;
            _cleanupTaskRepository = cleanupTaskRepository;
            _aiWasteScanRepository = aiWasteScanRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userManager = userManager;
        }

        public async Task<Response<DashboardAnalyticsResponse>> Handle(GetDashboardAnalyticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var startDate = GetStartDate(request.Period);

                var reportsQuery = _wasteReportRepository.GetTableNoTracking();
                if (startDate.HasValue)
                    reportsQuery = reportsQuery.Where(r => r.CreatedAt >= startDate.Value);

                var totalReports = await reportsQuery.CountAsync(cancellationToken);
                var pendingReports = await reportsQuery.CountAsync(r => r.Status == ReportStatus.Pending, cancellationToken);
                var inProgressReports = await reportsQuery.CountAsync(r => r.Status == ReportStatus.InProgress, cancellationToken);
                var completedReports = await reportsQuery.CountAsync(r => r.Status == ReportStatus.Done, cancellationToken);

                var aiScansQuery = _aiWasteScanRepository.GetTableNoTracking();
                if (startDate.HasValue)
                    aiScansQuery = aiScansQuery.Where(s => s.CreatedAt >= startDate.Value);
                var aiScans = await aiScansQuery.CountAsync(cancellationToken);

                var wasteCollectedQuery = _cleanupTaskRepository.GetTableNoTracking(t => t.CompletedAt.HasValue);
                if (startDate.HasValue)
                    wasteCollectedQuery = wasteCollectedQuery.Where(t => t.CompletedAt >= startDate.Value);
                var wasteCollectedInKg = await wasteCollectedQuery.SumAsync(t => t.FinalWeightInKg ?? 0m, cancellationToken);

                var totalUsersQuery = _userManager.Users.AsNoTracking();
                if (startDate.HasValue)
                    totalUsersQuery = totalUsersQuery.Where(u => u.CreatedAt >= startDate.Value);
                var totalUsers = await totalUsersQuery.CountAsync(cancellationToken);

                var workers = await GetUsersInRoleCountAsync(UserRole.Worker, startDate);
                var admins = await GetAdminsCountAsync(startDate);

                var categoriesQuery = _categoryRepository.GetTableNoTracking();
                if (startDate.HasValue)
                    categoriesQuery = categoriesQuery.Where(c => c.CreatedAt >= startDate.Value);
                var categories = await categoriesQuery.CountAsync(cancellationToken);

                var productsQuery = _productRepository.GetTableNoTracking();
                if (startDate.HasValue)
                    productsQuery = productsQuery.Where(p => p.CreatedAt >= startDate.Value);
                var products = await productsQuery.CountAsync(cancellationToken);

                var ordersQuery = _orderRepository.GetTableNoTracking();
                if (startDate.HasValue)
                    ordersQuery = ordersQuery.Where(o => o.OrderDate >= startDate.Value);
                var orders = await ordersQuery.CountAsync(cancellationToken);
                var itemsSold = await ordersQuery.SelectMany(o => o.OrderItems).SumAsync(oi => (int?)oi.Quantity, cancellationToken) ?? 0;

                var response = new DashboardAnalyticsResponse
                {
                    ReportsAnalytics = new ReportsAnalyticsResponse
                    {
                        TotalReports = totalReports,
                        PendingReports = pendingReports,
                        InProgressReports = inProgressReports,
                        CompletedReports = completedReports,
                        WasteCollectedInKg = wasteCollectedInKg,
                        AiScans = aiScans
                    },
                    UsersOverview = new UsersOverviewResponse
                    {
                        TotalUsers = totalUsers,
                        Workers = workers,
                        Admins = admins
                    },
                    ECommerceAnalytics = new ECommerceAnalyticsResponse
                    {
                        Categories = categories,
                        Products = products,
                        Orders = orders,
                        ItemsSold = itemsSold
                    }
                };

                return Success(response);
            }
            catch (Exception ex)
            {
                return BadRequest<DashboardAnalyticsResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<DashboardTrendsResponse>> Handle(GetDashboardTrendsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                bool byMonth = request.Period == LeaderboardPeriod.Yearly || request.Period == LeaderboardPeriod.AllTime;
                int iterations = request.Period switch
                {
                    LeaderboardPeriod.Weekly => 7,
                    LeaderboardPeriod.Monthly => 30,
                    LeaderboardPeriod.Yearly => 12,
                    _ => 12
                };

                DateTime startDate = byMonth
                    ? new DateTime(DateTime.UtcNow.AddMonths(-(iterations - 1)).Year, DateTime.UtcNow.AddMonths(-(iterations - 1)).Month, 1)
                    : DateTime.UtcNow.AddDays(-(iterations - 1)).Date;

                var reports = await _wasteReportRepository.GetTableNoTracking()
                    .Where(r => r.CreatedAt >= startDate)
                    .Select(r => r.CreatedAt)
                    .ToListAsync(cancellationToken);

                var orders = await _orderRepository.GetTableNoTracking()
                    .Where(o => o.OrderDate >= startDate)
                    .Select(o => o.OrderDate)
                    .ToListAsync(cancellationToken);

                var response = new DashboardTrendsResponse();

                for (int i = iterations - 1; i >= 0; i--)
                {
                    DateTime date = byMonth ? DateTime.UtcNow.AddMonths(-i) : DateTime.UtcNow.AddDays(-i).Date;
                    string label = byMonth ? date.ToString("MMM yyyy") : date.ToString("MMM dd");

                    response.ReportsTrend.Add(new TrendDataPoint
                    {
                        DateLabel = label,
                        Value = reports.Count(r => byMonth ? (r.Year == date.Year && r.Month == date.Month) : r.Date == date.Date)
                    });

                    response.OrdersTrend.Add(new TrendDataPoint
                    {
                        DateLabel = label,
                        Value = orders.Count(o => byMonth ? (o.Year == date.Year && o.Month == date.Month) : o.Date == date.Date)
                    });
                }

                return Success(response);
            }
            catch (Exception ex)
            {
                return BadRequest<DashboardTrendsResponse>($"An error occurred: {ex.Message}");
            }
        }

        private async Task<int> GetUsersInRoleCountAsync(UserRole role, DateTime? startDate)
        {
            var users = await _userManager.GetUsersInRoleAsync(role.ToString());
            var query = users.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt >= startDate.Value);

            return query.Count();
        }

        private async Task<int> GetAdminsCountAsync(DateTime? startDate)
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(UserRole.Admin.ToString());
            var superAdminUsers = await _userManager.GetUsersInRoleAsync(UserRole.SuperAdmin.ToString());

            var query = adminUsers
                .Concat(superAdminUsers)
                .GroupBy(u => u.Id)
                .Select(g => g.First())
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(u => u.CreatedAt >= startDate.Value);

            return query.Count();
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
