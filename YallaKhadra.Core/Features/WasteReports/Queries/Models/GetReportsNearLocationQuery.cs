using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Models {
    public class GetReportsNearLocationQuery : IRequest<Response<List<WasteReportResponse>>> {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public double RadiusInKm { get; set; } = 5.0; // Default 5 km
    }
}
