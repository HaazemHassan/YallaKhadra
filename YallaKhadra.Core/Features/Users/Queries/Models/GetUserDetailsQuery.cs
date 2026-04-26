using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class GetUserDetailsQuery : IRequest<Response<GetUserDetailsResponse>> {
        public int Id { get; set; }

        public GetUserDetailsQuery(int id) {
            Id = id;
        }
    }
}
