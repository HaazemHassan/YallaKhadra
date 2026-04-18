using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.Core.Features.Orders.Queries.Models {
    public class GetAllOrdersForAdminsQuery : IRequest<PaginatedResult<GetAllOrdersForAdminsResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UserFullName { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
