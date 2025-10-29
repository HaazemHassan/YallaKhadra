using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaKhadra.API.Bases;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;

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
    }
}
