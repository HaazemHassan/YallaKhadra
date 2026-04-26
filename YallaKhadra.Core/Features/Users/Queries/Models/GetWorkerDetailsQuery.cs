using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class GetWorkerDetailsQuery : IRequest<Response<GetWorkerDetailsResponse>> {
        public int Id { get; set; }

        public GetWorkerDetailsQuery(int id) {
            Id = id;
        }
    }
}
