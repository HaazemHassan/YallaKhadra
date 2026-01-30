using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Models {
    public class GetMyReportsQuery : IRequest<PaginatedResult<WasteReportResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public ReportStatus? Status { get; set; } // Optional filter by status
    }
}

