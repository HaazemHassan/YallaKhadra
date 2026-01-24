using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Products.Queries.Models;
using YallaKhadra.Core.Features.Products.Queries.Responses;

namespace YallaKhadra.Core.Features.Products.Queries.Handlers {
    public class ProductQueryHandler : ResponseHandler,
                                       IRequestHandler<GetProductByIdQuery, Response<GetProductByIdResponse>>,
                                       IRequestHandler<GetProductsPaginatedQuery, PaginatedResult<GetProductsPaginatedResponse>> {

        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductQueryHandler(
            IProductService productService,
            IProductRepository productRepository,
            IMapper mapper) {
            _productService = productService;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<Response<GetProductByIdResponse>> Handle(
            GetProductByIdQuery request,
            CancellationToken cancellationToken) {
            try {
                var product = await _productService.GetByIdAsync(request.Id, cancellationToken);

                if (product is null)
                    return NotFound<GetProductByIdResponse>("Product not found.");

                var productResponse = _mapper.Map<GetProductByIdResponse>(product);
                return Success(productResponse);
            }
            catch (Exception ex) {
                return BadRequest<GetProductByIdResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<GetProductsPaginatedResponse>> Handle(
            GetProductsPaginatedQuery request,
            CancellationToken cancellationToken) {
            try {
                var productsQueryable = _productRepository
                    .GetTableNoTracking()
                    .Include(p => p.Images)
                    .AsQueryable();

                if (request.CategoryId.HasValue)
                    productsQueryable = productsQueryable.Where(p => p.CategoryId == request.CategoryId.Value);


                productsQueryable = productsQueryable.OrderByDescending(p => p.CreatedAt);

                var productsPaginatedResult = await productsQueryable
                    .ProjectTo<GetProductsPaginatedResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return productsPaginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<GetProductsPaginatedResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
