using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Models {
    public class GetReportsNearLocationQuery : IRequest<PaginatedResult<WasteReportResponse>> {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double RadiusInKm { get; set; } = 5.0; // Default 5 km
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
