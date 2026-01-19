using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Models;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Responses;

namespace YallaKhadra.Core.Features.PointsTransactions.Queries.Handlers {
    public class PointsTransactionQueryHandler : ResponseHandler,
                                                   IRequestHandler<GetMyTransactionsQuery, PaginatedResult<PointsTransactionResponse>>,
                                                   IRequestHandler<GetMyBalanceQuery, Response<UserBalanceResponse>> {

        private readonly IPointsTransactionRepository _pointsTransactionRepository;
        private readonly IPointsTransactionService _pointsTransactionService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public PointsTransactionQueryHandler(
            IPointsTransactionRepository pointsTransactionRepository,
            IPointsTransactionService pointsTransactionService,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _pointsTransactionRepository = pointsTransactionRepository;
            _pointsTransactionService = pointsTransactionService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<PointsTransactionResponse>> Handle(
            GetMyTransactionsQuery request,
            CancellationToken cancellationToken) {
            try {
                // Get current user ID
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return PaginatedResult<PointsTransactionResponse>.Failure("User is not authenticated.");

                // Get user's transactions with pagination
                var transactionsQuery = _pointsTransactionRepository
                    .GetTableNoTracking(t => t.UserId == userId.Value)
                    .OrderByDescending(t => t.CreatedAt);

                // Project to DTO and paginate
                var paginatedResult = await transactionsQuery
                    .ProjectTo<PointsTransactionResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<PointsTransactionResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<UserBalanceResponse>> Handle(
            GetMyBalanceQuery request,
            CancellationToken cancellationToken) {
            try {
                // Get current user ID
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<UserBalanceResponse>("User is not authenticated.");

                // Get user's balance
                var balance = await _pointsTransactionService.GetUserBalanceAsync(
                    userId.Value,
                    cancellationToken);

                // Get user's transactions for statistics
                var transactions = await _pointsTransactionService.GetUserTransactionsAsync(
                    userId.Value,
                    cancellationToken);

                var response = new UserBalanceResponse {
                    PointsBalance = balance,
                    TotalEarned = transactions.Where(t => t.TransactionType == PointsTransactionType.Earn).Sum(t => t.Points),
                    TotalSpent = transactions.Where(t => t.TransactionType == PointsTransactionType.Spend).Sum(t => t.Points),
                    TransactionCount = transactions.Count
                };

                return Success(response, message: $"Current balance: {balance} points");
            }
            catch (Exception ex) {
                return BadRequest<UserBalanceResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
