using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Products.Commands.Responses;

namespace YallaKhadra.Core.Features.Products.Commands.RequestModels {
    public class AddProductCommand : IRequest<Response<AddProductResponse>> {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int PointsCost { get; set; }
        public int Stock { get; set; }
        public List<IFormFile> Images { get; set; } = new();
    }
}
