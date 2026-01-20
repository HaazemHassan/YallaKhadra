using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Models {
    public class GetPendingReportsQuery : IRequest<PaginatedResult<WasteReportResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
