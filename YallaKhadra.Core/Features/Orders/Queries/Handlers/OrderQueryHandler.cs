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
                                      IRequestHandler<GetAllOrdersForAdminsQuery, PaginatedResult<GetAllOrdersForAdminsResponse>>,
                                      IRequestHandler<GetOrderByIdQuery, Response<GetOrderDetailsResponse>>,
                                      IRequestHandler<GetOrderDetailsForAdminsQuery, Response<GetOrderDetailsResponse>> {
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

        public async Task<PaginatedResult<GetAllOrdersForAdminsResponse>> Handle(
            GetAllOrdersForAdminsQuery request,
            CancellationToken cancellationToken) {
            try {
                var normalizedUserFullName = request.UserFullName?.Trim();

                var ordersQueryable = _orderRepository
                    .GetTableNoTracking(order =>
                        (string.IsNullOrWhiteSpace(normalizedUserFullName) ||
                         ((order.ApplicationUser.FirstName + " " + order.ApplicationUser.LastName).Trim()).Contains(normalizedUserFullName)) &&
                        (!request.Status.HasValue || order.Status == request.Status.Value))
                    .OrderByDescending(order => order.OrderDate)
                    .Select(order => new GetAllOrdersForAdminsResponse {
                        Id = order.Id,
                        UserFullName = (order.ApplicationUser.FirstName + " " + order.ApplicationUser.LastName).Trim(),
                        Date = order.OrderDate,
                        ItemsCount = order.OrderItems.Count,
                        TotalPoints = order.TotalPoints,
                        Status = order.Status
                    });

                var ordersPaginatedResult = await ordersQueryable
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return ordersPaginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<GetAllOrdersForAdminsResponse>.Failure($"An error occurred: {ex.Message}");
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
                    .ProjectTo<GetOrderDetailsResponse>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (order is null)
                    return NotFound<GetOrderDetailsResponse>($"Order with ID {request.Id} not found");

                return Success(order);
            }
            catch (Exception ex) {
                return BadRequest<GetOrderDetailsResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetOrderDetailsResponse>> Handle(
            GetOrderDetailsForAdminsQuery request,
            CancellationToken cancellationToken) {
            try {
                var order = await _orderRepository
                    .GetTableNoTracking(o => o.Id == request.Id)
                    .ProjectTo<GetOrderDetailsResponse>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(cancellationToken);

                if (order is null)
                    return NotFound<GetOrderDetailsResponse>($"Order with ID {request.Id} not found");

                return Success(order);
            }
            catch (Exception ex) {
                return BadRequest<GetOrderDetailsResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
