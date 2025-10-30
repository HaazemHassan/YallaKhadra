using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Reports.Queries.Responses;

namespace YallaKhadra.Core.Features.Reports.Queries.Models
{
    public class GetMyReportsPaginatedQuery : IRequest<PaginatedResult<GetMyReportsPaginatedResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public ReportStatus? Status { get; set; }
    }
}
