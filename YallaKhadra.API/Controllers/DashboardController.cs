using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Dashboard.Queries.Models;

namespace YallaKhadra.API.Controllers
{
    [Authorize(Roles = $"{nameof(UserRole.Admin)},{nameof(UserRole.SuperAdmin)}")]
    public class DashboardController : BaseController
    {
        [HttpGet("analytics")]
        public async Task<IActionResult> GetDashboardAnalytics([FromQuery] GetDashboardAnalyticsQuery query)
        {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }

        [HttpGet("trends")]
        public async Task<IActionResult> GetDashboardTrends([FromQuery] GetDashboardTrendsQuery query)
        {
            var result = await Mediator.Send(query);
            return NewResult(result);
        }
    }
}
