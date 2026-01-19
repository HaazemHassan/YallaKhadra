using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services {
    public class PointsTransactionService : IPointsTransactionService {
        private readonly IPointsTransactionRepository _pointsTransactionRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public PointsTransactionService(IPointsTransactionRepository pointsTransactionRepository, UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork)
        {
            _pointsTransactionRepository = pointsTransactionRepository;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceOperationResult<PointsTransaction>> EarnPointsAsync(int userId,int points,PointsSourceType sourceType,int sourceId,
            CancellationToken cancellationToken = default) {
            try {
                // Validate points
                if (points <= 0)
                    return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed, "Points must be greater than zero.")!;

                // Get user
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                    return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.NotFound,"User not found.")!;

                // Begin transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try {
                    var pointsTransaction = new PointsTransaction {
                        UserId = userId,
                        Points = points,
                        TransactionType = PointsTransactionType.Earn,
                        pointsSource = sourceType,
                        SourceId = sourceId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _pointsTransactionRepository.AddAsync(pointsTransaction);

                    // Update user's balance
                    user.PointsBalance += points;
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded) {
                        await _unitOfWork.RollbackAsync();
                        return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,"Failed to update user balance.")!;
                    }

                    // Commit transaction
                    await _unitOfWork.CommitAsync();

                    return ServiceOperationResult<PointsTransaction>.Success(pointsTransaction)!;
                }
                catch {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex) {
                return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,$"Failed to earn points: {ex.Message}")!;
            }
        }

        public async Task<ServiceOperationResult<PointsTransaction>> SpendPointsAsync(int userId,int points,PointsSourceType sourceType,int sourceId,
            CancellationToken cancellationToken = default) {
            try {
                // Validate points
                if (points <= 0)
                    return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,"Points must be greater than zero.")!;

                // Get user
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user is null)
                    return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.NotFound,"User not found.")!;


                if (user.PointsBalance < points)
                    return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,"Insufficient points balance.")!;

                // Begin transaction
                using var transaction = await _unitOfWork.BeginTransactionAsync();
                try {
                    var pointsTransaction = new PointsTransaction {
                        UserId = userId,
                        Points = points,
                        TransactionType = PointsTransactionType.Spend,
                        pointsSource = sourceType,
                        SourceId = sourceId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _pointsTransactionRepository.AddAsync(pointsTransaction);

                    // Update user balance
                    user.PointsBalance -= points;
                    var updateResult = await _userManager.UpdateAsync(user);

                    if (!updateResult.Succeeded) {
                        await _unitOfWork.RollbackAsync();
                        return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,"Failed to update user balance.")!;
                    }

                    // Commit transaction
                    await _unitOfWork.CommitAsync();

                    return ServiceOperationResult<PointsTransaction>.Success(pointsTransaction)!;
                }
                catch {
                    await _unitOfWork.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex) {
                return ServiceOperationResult<PointsTransaction>.Failure(ServiceOperationStatus.Failed,$"Failed to spend points: {ex.Message}")!;
            }
        }

        public async Task<List<PointsTransaction>> GetUserTransactionsAsync(
            int userId,
            CancellationToken cancellationToken = default) {
            return await _pointsTransactionRepository
                .GetTableNoTracking(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetUserBalanceAsync(int userId, CancellationToken cancellationToken = default) {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.PointsBalance ?? 0;
        }
    }
}
