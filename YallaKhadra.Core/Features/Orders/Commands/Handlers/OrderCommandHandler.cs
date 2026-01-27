using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Orders.Commands.RequestModels;
using YallaKhadra.Core.Features.Orders.Commands.Responses;

namespace YallaKhadra.Core.Features.Orders.Commands.Handlers {
    public class PlaceOrderCommandHandler : ResponseHandler,
                                            IRequestHandler<PlaceOrderCommand, Response<PlaceOrderResponse>> {
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public PlaceOrderCommandHandler(
            IOrderService orderService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _orderService = orderService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<PlaceOrderResponse>> Handle(
            PlaceOrderCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized<PlaceOrderResponse>("User is not authenticated.");

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try {
                // Call service to handle all business logic
                var result = await _orderService.PlaceOrderAsync(
                    userId.Value,
                    request.FullName,
                    request.PhoneNumber,
                    request.City,
                    request.StreetAddress,
                    request.BuildingNumber,
                    request.Landmark,
                    request.ShippingNotes,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded) {
                    await transaction.RollbackAsync(cancellationToken);
                    return BadRequest<PlaceOrderResponse>(result.ErrorMessage ?? "Order placement failed.");
                }

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);

                var response = _mapper.Map<PlaceOrderResponse>(result.Data);

                return Created(response);
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest<PlaceOrderResponse>($"Order placement failed: {ex.Message}");
            }
        }
    }
}
