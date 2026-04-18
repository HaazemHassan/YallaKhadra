using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Orders.Commands.RequestModels {
    public class AdvanceOrderStatusCommand : IRequest<Response> {
        public int OrderId { get; set; }
    }
}
