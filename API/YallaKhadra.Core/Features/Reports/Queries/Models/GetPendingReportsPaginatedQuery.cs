using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Reports.Queries.Responses;

namespace YallaKhadra.Core.Features.Reports.Queries.Models
{
    public class GetPendingReportsPaginatedQuery : IRequest<PaginatedResult<GetPendingReportsPaginatedResponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
