using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Carts.Queries.Responses;

namespace YallaKhadra.Core.Features.Carts.Queries.Models {
    public class GetCartQuery : IRequest<Response<GetCartResponse>> {
    }
}
