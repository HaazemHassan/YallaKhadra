using YallaKhadra.Core.Features.Users.Queries.Responses;
using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models;

public class GetUserByUsernameQuery : IRequest<Response<GetUserByUsernameResponse>> {
    public string Username { get; set; }
}