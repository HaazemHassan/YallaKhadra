using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Reports.Commands.RequestsModels
{
    public class ReviewReportCommand : IRequest<Response<string>>
    {
        public int ReportId { get; set; }
        public bool IsApproved { get; set; }
        public string? Notes { get; set; }
    }
}
