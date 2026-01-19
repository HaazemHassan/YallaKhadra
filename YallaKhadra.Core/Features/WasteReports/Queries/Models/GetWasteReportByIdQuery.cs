using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Models {
    public class GetWasteReportByIdQuery : IRequest<Response<WasteReportResponse>> {
        public int Id { get; set; }
    }
}
