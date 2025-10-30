using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;
using YallaKhadra.Core.Features.Reports.Queries.Models;

namespace YallaKhadra.API.Controllers
{
    [Route("api/[controller]")]
    public class ReportController : BaseController
    {

        [HttpPost("add-report")]
        [Authorize]
        public async Task<IActionResult> AddReport([FromForm] AddReportCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet("my-reports")]
        [Authorize]
        public async Task<IActionResult> GetMyReports([FromQuery] GetMyReportsPaginatedQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }


        [HttpGet("pending")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> GetPendingReports([FromQuery] GetPendingReportsPaginatedQuery query)
        {
            var result = await Mediator.Send(query);
            return Ok(result);
        }


        [HttpPut("review")]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<IActionResult> ReviewReport([FromBody] ReviewReportCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}
