using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Carts.Commands.Responses;

namespace YallaKhadra.Core.Features.Carts.Commands.RequestModels {
    public class AddToCartCommand : IRequest<Response<AddToCartResponse>> {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
