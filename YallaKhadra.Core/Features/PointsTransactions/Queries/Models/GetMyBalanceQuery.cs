using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Responses;

namespace YallaKhadra.Core.Features.PointsTransactions.Queries.Models {
    public class GetMyBalanceQuery : IRequest<Response<UserBalanceResponse>> {
    }
}
