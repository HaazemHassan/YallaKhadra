using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models {
    public class CheckEmailAvailabilityQuery : IRequest<Response<bool>> {
        public string Email { get; set; }
    }
}
