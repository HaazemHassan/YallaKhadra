using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Features.Users.Queries.Models;

public class SearchUsersQuery : IRequest<Response<List<SearchUsersResponse>>> {
    public string Username { get; set; }
}