using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Orders.Queries.Models;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.Core.Features.Orders.Queries.Handlers {
    public class OrderQueryHandler : ResponseHandler,
                                      IRequestHandler<GetMyOrdersQuery, PaginatedResult<GetMyOrdersResponse>>,
                                      IRequestHandler<GetOrderByIdQuery, Response<GetOrderDetailsResponse>> {
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public OrderQueryHandler(
            IOrderRepository orderRepository,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _orderRepository = orderRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<GetMyOrdersResponse>> Handle(
            GetMyOrdersQuery request,
            CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (userId is null)
                    return PaginatedResult<GetMyOrdersResponse>.Failure("User is not authenticated");

                var ordersQueryable = _orderRepository
                    .GetTableNoTracking(o => o.UserId == userId.Value)
                    .OrderByDescending(o => o.OrderDate);

                var ordersPaginatedResult = await ordersQueryable
                    .ProjectTo<GetMyOrdersResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return ordersPaginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<GetMyOrdersResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetOrderDetailsResponse>> Handle(
            GetOrderByIdQuery request,
            CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (userId is null)
                    return Unauthorized<GetOrderDetailsResponse>("User is not authenticated");

                var order = await _orderRepository
                    .GetTableNoTracking(o => o.Id == request.Id && o.UserId == userId.Value)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                            .ThenInclude(p => p.Images)
                    .Include(o => o.ShippingDetails)
                    .FirstOrDefaultAsync(cancellationToken);

                if (order is null)
                    return NotFound<GetOrderDetailsResponse>($"Order with ID {request.Id} not found");

                var response = _mapper.Map<GetOrderDetailsResponse>(order);

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<GetOrderDetailsResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
