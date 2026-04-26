using MediatR;
using YallaKhadra.Core.Attributes;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Commands.Responses;

namespace YallaKhadra.Core.Features.Users.Commands.RequestModels {
    public class ToggleUserLockCommand : IRequest<Response<ToggleUserLockResponse>> {
        [SwaggerExclude]
        public int Id { get; set; }
    }
}
