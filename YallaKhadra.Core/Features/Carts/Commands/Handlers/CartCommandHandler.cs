using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Features.Carts.Commands.RequestModels;
using YallaKhadra.Core.Features.Carts.Commands.Responses;

namespace YallaKhadra.Core.Features.Carts.Commands.Handlers {
    public class CartCommandHandler : ResponseHandler,
                                       IRequestHandler<AddToCartCommand, Response<AddToCartResponse>>,
                                       IRequestHandler<UpdateCartItemQuantityCommand, Response<UpdateCartItemQuantityResponse>>,
                                       IRequestHandler<ToggleCartItemSelectionCommand, Response<ToggleCartItemSelectionResponse>>,
                                       IRequestHandler<RemoveFromCartCommand, Response>,
                                       IRequestHandler<ClearCartCommand, Response> {

        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CartCommandHandler(
            ICartRepository cartRepository,
            ICartItemRepository cartItemRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService) {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Response<AddToCartResponse>> Handle(
            AddToCartCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized<AddToCartResponse>("User is not authenticated.");

            var product = await _productRepository
                .GetTableNoTracking(p => p.Id == request.ProductId)
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null || !product.IsActive)
                return NotFound<AddToCartResponse>("Product not found.");


            if (product.Stock < request.Quantity)
                return BadRequest<AddToCartResponse>("Out of stock.");


            var cart = await _cartRepository.GetCartByUserIdWithItemsAsync(userId.Value, cancellationToken);

            if (cart is null) {
                cart = new Cart {
                    UserId = userId.Value,
                    CreatedAt = DateTime.UtcNow
                };
                await _cartRepository.AddAsync(cart);
            }

            cart.AddOrUpdateItem(product, request.Quantity);

            await _unitOfWork.SaveChangesAsync();

            var item = cart.Items.First(i => i.ProductId == product.Id);
            var response = new AddToCartResponse {
                CartItemId = item.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                Quantity = item.Quantity,
                PointsCost = item.PointsCost,
                TotalPoints = item.Quantity * item.PointsCost
            };

            return Created(response);
        }

        public async Task<Response<UpdateCartItemQuantityResponse>> Handle(
            UpdateCartItemQuantityCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized<UpdateCartItemQuantityResponse>("User is not authenticated.");

            var cartItem = await _cartItemRepository
                .GetTableAsTracking(ci => ci.Id == request.CartItemId)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(cancellationToken);

            if (cartItem is null)
                return NotFound<UpdateCartItemQuantityResponse>("Cart item not found.");

            if (!cartItem.Cart.IsOwnedBy(userId.Value))
                return Unauthorized<UpdateCartItemQuantityResponse>("You don't have permission to update this cart item.");

            var product = await _productRepository
                .GetTableNoTracking(p => p.Id == cartItem.ProductId)
                .FirstOrDefaultAsync(cancellationToken);

            if (product is null || !product.IsActive)
                return NotFound<UpdateCartItemQuantityResponse>("Product not found.");

            try {
                cartItem.UpdateQuantity(request.Quantity, product.Stock);
                cartItem.Cart.MarkAsUpdated();
            }
            catch (InvalidOperationException ex) {
                return BadRequest<UpdateCartItemQuantityResponse>(ex.Message);
            }

            await _cartItemRepository.UpdateAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var response = new UpdateCartItemQuantityResponse {
                CartItemId = cartItem.Id,
                Quantity = cartItem.Quantity,
                PointsCost = cartItem.PointsCost,
                TotalPoints = cartItem.CalculateTotalPoints()
            };

            return Success(response);
        }

        public async Task<Response<ToggleCartItemSelectionResponse>> Handle(
            ToggleCartItemSelectionCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized<ToggleCartItemSelectionResponse>("User is not authenticated.");

            var cartItem = await _cartItemRepository
                .GetTableAsTracking(ci => ci.Id == request.CartItemId)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(cancellationToken);

            if (cartItem is null)
                return NotFound<ToggleCartItemSelectionResponse>("Cart item not found.");

            if (!cartItem.Cart.IsOwnedBy(userId.Value))
                return Unauthorized<ToggleCartItemSelectionResponse>("You don't have permission to update this cart item.");

            cartItem.ToggleSelection(request.IsSelected);
            cartItem.Cart.MarkAsUpdated();

            _cartItemRepository.UpdateWithoutSave(cartItem);
            await _unitOfWork.SaveChangesAsync();

            var response = new ToggleCartItemSelectionResponse {
                CartItemId = cartItem.Id,
                IsSelected = cartItem.IsSelected
            };

            return Success(response);
        }

        public async Task<Response> Handle(
            RemoveFromCartCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized("User is not authenticated.");

            var cartItem = await _cartItemRepository
                .GetTableAsTracking(ci => ci.Id == request.CartItemId)
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(cancellationToken);

            if (cartItem is null)
                return NotFound("Cart item not found.");

            if (!cartItem.Cart.IsOwnedBy(userId.Value))
                return Unauthorized("You don't have permission to remove this cart item.");

            cartItem.Cart.MarkAsUpdated();

            _cartItemRepository.DeleteWithoutSave(cartItem);
            await _unitOfWork.SaveChangesAsync();

            return Success("Item removed from cart successfully.");
        }

        public async Task<Response> Handle(
            ClearCartCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (userId is null)
                return Unauthorized("User is not authenticated.");

            var cart = await _cartRepository
                .GetTableAsTracking(c => c.UserId == userId.Value)
                .Include(c => c.Items)
                .FirstOrDefaultAsync(cancellationToken);

            if (cart is null)
                return NotFound("Cart not found.");

            if (!cart.IsOwnedBy(userId.Value))
                return Unauthorized("You don't have permission to clear this cart.");

            cart.ClearItems();

            _cartRepository.UpdateWithoutSave(cart);
            await _unitOfWork.SaveChangesAsync();

            return Success("Cart cleared successfully.");
        }
    }
}
