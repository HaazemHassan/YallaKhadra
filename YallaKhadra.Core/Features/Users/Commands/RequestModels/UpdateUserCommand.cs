using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.RequestModels {
    public class UpdateUserCommand : IRequest<Response> {

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }

    }
}
