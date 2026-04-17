using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Categories.Queries.Responses;

namespace YallaKhadra.Core.Features.Categories.Queries.Models {
    public class GetCategoriesPaginatedQuery : IRequest<PaginatedResult<GetCategoriesPaginatedResponse>> {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
