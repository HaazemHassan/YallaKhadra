using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;

namespace YallaKhadra.Services.Services.Ecommerce_services {
    public class OrderService : IOrderService {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPointsTransactionService _pointsTransactionService;
        private readonly IPointsTransactionRepository _pointsTransactionRepository;

        public OrderService(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IPointsTransactionService pointsTransactionService,
            IPointsTransactionRepository pointsTransactionRepository) {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _pointsTransactionService = pointsTransactionService;
            _pointsTransactionRepository = pointsTransactionRepository;
        }

        public async Task<ServiceOperationResult<Order>> PlaceOrderAsync(
            int userId,
            string fullName,
            string phoneNumber,
            string city,
            string streetAddress,
            string? buildingNumber,
            string? landmark,
            string? shippingNotes,
            CancellationToken cancellationToken = default) {

            // 1. Get cart with selected items
            var cart = await _cartRepository
                    .GetTableAsTracking(c => c.UserId == userId)
                    .Include(c => c.Items.Where(ci => ci.IsSelected))
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(cancellationToken);

            if (cart is null)
                return ServiceOperationResult<Order>.Failure(
                    ServiceOperationStatus.NotFound,
                    "Cart not found for the user.")!;

            // 2. Validate selected items exist
            if (!cart.Items.Any())
                return ServiceOperationResult<Order>.Failure(
                    ServiceOperationStatus.Failed,
                    "No items selected for checkout.")!;

            // 3. Check if products are still available
            foreach (var cartItem in cart.Items) {
                if (!cartItem.Product.IsActive) {
                    return ServiceOperationResult<Order>.Failure(
                         ServiceOperationStatus.Failed, "Some products are no longer available")!;
                }
            }

            // 4. Check for price changes
            foreach (var cartItem in cart.Items)
                if (cartItem.HasPriceChanged(cartItem.Product.PointsCost))
                    return ServiceOperationResult<Order>.Failure(
                        ServiceOperationStatus.Failed, "Some product prices have changed. Please review your cart")!;

            // 5. Create order items and validate stock
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.Items) {
                if (!cartItem.Product.HasSufficientStock(cartItem.Quantity))
                    return ServiceOperationResult<Order>.Failure(
                        ServiceOperationStatus.Failed,
                        $"Insufficient stock for product: {cartItem.Product.Name}. Available: {cartItem.Product.Stock}, Requested: {cartItem.Quantity}")!;

                var orderItem = OrderItem.CreateFromCartItem(cartItem);
                orderItems.Add(orderItem);
            }

            int totalPoints = orderItems.Sum(item => item.CalculateTotalPoints());

            // 7. Validate user has enough points
            var userBalance = await _pointsTransactionService.GetUserBalanceAsync(
                userId,
                cancellationToken);

            if (userBalance < totalPoints)
                return ServiceOperationResult<Order>.Failure(
                    ServiceOperationStatus.Failed,
                    $"Insufficient points balance. Required: {totalPoints}, Available: {userBalance}")!;

            // 8. Deduct points from user
            var pointsResult = await _pointsTransactionService.SpendPointsAsync(
                userId,
                totalPoints,
                PointsSourceType.EcoStore,
                0,
                cancellationToken);

            if (pointsResult.Status != ServiceOperationStatus.Succeeded)
                return ServiceOperationResult<Order>.Failure(
                    pointsResult.Status,
                    pointsResult.ErrorMessage ?? "Failed to deduct points.")!;

            // 9. Reduce stock
            foreach (var cartItem in cart.Items) {
                cartItem.Product.ReduceStock(cartItem.Quantity);
                _productRepository.UpdateWithoutSave(cartItem.Product);
            }

            // 10. Create shipping details
            var shippingDetails = new OrderShippingDetails {
                FullName = fullName,
                PhoneNumber = phoneNumber,
                City = city,
                StreetAddress = streetAddress,
                BuildingNumber = buildingNumber,
                Landmark = landmark,
                ShippingNotes = shippingNotes
            };

            var order = Order.CreateOrder(userId, orderItems, shippingDetails);

            var createdOrder = await _orderRepository.AddAsync(order);    //must save because we need the order ID

            // 12. Update points transaction with order ID
            var pointsTransaction = pointsResult.Data;
            if (pointsTransaction is not null) {
                pointsTransaction.SourceId = createdOrder.Id;
                _pointsTransactionRepository.UpdateWithoutSave(pointsTransaction);
            }

            // 13. Clear selected items from cart
            foreach (var cartItem in cart.Items)
                _cartItemRepository.DeleteWithoutSave(cartItem);


            // 14. Return created order
            return ServiceOperationResult<Order>.Success(createdOrder)!;
        }

        public async Task<ServiceOperationResult> CancelOrderAsync(
            int orderId,
            int userId,
            CancellationToken cancellationToken = default) {

            // 1. Get order with all related data
            var order = await _orderRepository
                .GetTableAsTracking(o => o.Id == orderId && o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(cancellationToken);

            if (order is null)
                return ServiceOperationResult.Failure(
                    ServiceOperationStatus.NotFound,
                    $"Order with ID {orderId} not found.");

            // 2. Check if order can be canceled
            if (!order.CanBeCanceled())
                return ServiceOperationResult.Failure(
                    ServiceOperationStatus.Failed,
                    $"Order cannot be canceled. Current status: {order.Status}");

            // 3. Cancel the order (updates status to Canceled)
            order.CancelOrder();
            _orderRepository.UpdateWithoutSave(order);

            // 4. Restore product stock
            foreach (var orderItem in order.OrderItems) {
                orderItem.Product.IncreaseStock(orderItem.Quantity);
                _productRepository.UpdateWithoutSave(orderItem.Product);
            }

            // 5. Refund points to user
            var refundResult = await _pointsTransactionService.EarnPointsAsync(
                userId,
                order.TotalPoints,
                PointsSourceType.EcoStore,
                order.Id,
                cancellationToken);

            if (refundResult.Status != ServiceOperationStatus.Succeeded)
                return ServiceOperationResult.Failure(
                    refundResult.Status,
                    refundResult.ErrorMessage ?? "Failed to refund points.");

            return ServiceOperationResult.Success();
        }
    }
}
