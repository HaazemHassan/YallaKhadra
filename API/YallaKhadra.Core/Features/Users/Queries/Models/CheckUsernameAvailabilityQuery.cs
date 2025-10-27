using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class CheckUsernameAvailabilityQuery : IRequest<Response<bool>> {
        public string Username { get; set; }
    }
}
