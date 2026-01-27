using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.Core.Features.Orders.Queries.Models {
    public class GetMyOrdersQuery : IRequest<PaginatedResult<GetMyOrdersResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
