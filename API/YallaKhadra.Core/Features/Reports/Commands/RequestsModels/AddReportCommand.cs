using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Reports.Commands.RequestsModels
{
    public class AddReportCommand : IRequest<Response<string>>
    {
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public IList<IFormFile>? Photos { get; set; }
    }
}
