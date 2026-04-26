using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class GetUsersByRoleQuery : IRequest<PaginatedResult<GetUsersByRoleResponse>> {
        public UserRole Role { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
