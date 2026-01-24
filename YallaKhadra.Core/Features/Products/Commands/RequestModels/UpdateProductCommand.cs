using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Products.Commands.Responses;

namespace YallaKhadra.Core.Features.Products.Commands.RequestModels {
    public class UpdateProductCommand : IRequest<Response<UpdateProductResponse>> {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? PointsCost { get; set; }
        public int? Stock { get; set; }
        public bool? IsActive { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
