using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Attributes;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.RequestModels {
    public class UpdateUserCommand : IRequest<Response> {
        [SwaggerExclude]
        public int Id { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
