using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class GetUsersPaginatedQuery : IRequest<PaginatedResult<GetUsersPaginatedResponse>> {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
