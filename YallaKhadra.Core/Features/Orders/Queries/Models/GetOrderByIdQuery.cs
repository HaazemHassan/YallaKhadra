using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.Core.Features.Orders.Queries.Models {
    public class GetOrderByIdQuery : IRequest<Response<GetOrderDetailsResponse>> {
        public int Id { get; set; }
    }
}
