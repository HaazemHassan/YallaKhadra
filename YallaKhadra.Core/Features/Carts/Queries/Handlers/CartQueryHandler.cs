using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Carts.Queries.Models;
using YallaKhadra.Core.Features.Carts.Queries.Responses;

namespace YallaKhadra.Core.Features.Carts.Queries.Handlers {
    public class CartQueryHandler : ResponseHandler,
                                     IRequestHandler<GetCartQuery, Response<GetCartResponse>> {

        private readonly ICartRepository _cartRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public CartQueryHandler(
            ICartRepository cartRepository,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _cartRepository = cartRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<GetCartResponse>> Handle(
            GetCartQuery request,
            CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (userId is null)
                    return Unauthorized<GetCartResponse>("User is not authenticated.");

                var cart = await _cartRepository
                    .GetTableNoTracking()
                    .Include(c => c.Items)
                        .ThenInclude(ci => ci.Product)
                            .ThenInclude(p => p.Images)
                    .FirstOrDefaultAsync(c => c.UserId == userId.Value, cancellationToken);

                if (cart is null)
                    return NotFound<GetCartResponse>("Cart not found.");

                var cartResponse = _mapper.Map<GetCartResponse>(cart);
                return Success(cartResponse);
            }
            catch (Exception ex) {
                return BadRequest<GetCartResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
