using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.PointsEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IPointsTransactionService {
        Task<ServiceOperationResult<PointsTransaction>> EarnPointsAsync(int userId,int points, PointsSourceType sourceType,int sourceId,
            CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<PointsTransaction>> SpendPointsAsync(int userId,int points,PointsSourceType sourceType,int sourceId,
            CancellationToken cancellationToken = default);

        Task<List<PointsTransaction>> GetUserTransactionsAsync(int userId, CancellationToken cancellationToken = default); 

        Task<int> GetUserBalanceAsync(int userId, CancellationToken cancellationToken = default);
    }
}
