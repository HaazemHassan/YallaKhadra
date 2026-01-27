using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;

namespace YallaKhadra.Core.Features.Orders.Commands.Handlers {
    public class CancelOrderCommandHandler : ResponseHandler,
                                             IRequestHandler<CancelOrderCommand, Response> {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CancelOrderCommandHandler(
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Response> Handle(
            CancelOrderCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized("User is not authenticated");

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try {
                var result = await _orderService.CancelOrderAsync(
                    request.OrderId,
                    userId.Value,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded) {
                    await transaction.RollbackAsync(cancellationToken);
                    return BadRequest(result.ErrorMessage ?? "Order cancellation failed.");
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);

                return Success($"Order #{request.OrderId} has been canceled successfully.");
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest($"Failed to cancel order: {ex.Message}");
            }
        }
    }
}
