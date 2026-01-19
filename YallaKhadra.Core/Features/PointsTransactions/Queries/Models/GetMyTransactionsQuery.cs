using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Responses;

namespace YallaKhadra.Core.Features.PointsTransactions.Queries.Models {
    public class GetMyTransactionsQuery : IRequest<PaginatedResult<PointsTransactionResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
