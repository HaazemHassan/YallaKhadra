using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Products.Commands.RequestModels {
    public class DeleteProductCommand : IRequest<Response> {
        public int Id { get; set; }
    }
}
