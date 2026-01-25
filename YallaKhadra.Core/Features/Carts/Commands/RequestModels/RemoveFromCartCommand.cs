using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Carts.Commands.RequestModels {
    public class RemoveFromCartCommand : IRequest<Response> {
        public int CartItemId { get; set; }
    }
}
