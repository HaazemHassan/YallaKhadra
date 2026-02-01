using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Commands.RequestModels {
    public class CreateWasteReportCommand : IRequest<Response<WasteReportResponse>> {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Address { get; set; } = null!;
        public int WasteType { get; set; }
        public List<IFormFile> Images { get; set; } = null!;
    }
}
