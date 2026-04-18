using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Orders.Commands.Handlers {
    public class AdvanceOrderStatusCommandHandler : ResponseHandler,
                                                    IRequestHandler<AdvanceOrderStatusCommand, Response> {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AdvanceOrderStatusCommandHandler(
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Response> Handle(
            AdvanceOrderStatusCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized("User is not authenticated");

            var isAdmin = _currentUserService.IsInRole(UserRole.Admin) ||
                          _currentUserService.IsInRole(UserRole.SuperAdmin);

            if (!isAdmin)
                return Forbid("Only admins can change order status.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try {
                var result = await _orderService.AdvanceOrderStatusAsync(
                    request.OrderId,
                    cancellationToken);

                if (result.Status == ServiceOperationStatus.NotFound) {
                    await transaction.RollbackAsync(cancellationToken);
                    return NotFound(result.ErrorMessage ?? "Order not found.");
                }

                if (result.Status != ServiceOperationStatus.Succeeded) {
                    await transaction.RollbackAsync(cancellationToken);
                    return BadRequest(result.ErrorMessage ?? "Order status update failed.");
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);

                return Success(message: $"Order #{request.OrderId} status updated to {result.Data}.");
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest($"Failed to update order status: {ex.Message}");
            }
        }
    }
}
