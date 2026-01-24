using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Products.Queries.Responses;

namespace YallaKhadra.Core.Features.Products.Queries.Models {
    public class GetProductsPaginatedQuery : IRequest<PaginatedResult<GetProductsPaginatedResponse>> {
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
