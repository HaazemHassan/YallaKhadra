using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class GetUserByIdQuery : IRequest<Response<GetUserByIdResponse>> {
        public int Id { get; set; }
    }
}
