using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Products.Commands.RequestModels;
using YallaKhadra.Core.Features.Products.Commands.Responses;

namespace YallaKhadra.Core.Features.Products.Commands.Handlers {
    public class ProductCommandHandler : ResponseHandler,
                                         IRequestHandler<AddProductCommand, Response<AddProductResponse>>,
                                         IRequestHandler<UpdateProductCommand, Response<UpdateProductResponse>>,
                                         IRequestHandler<DeleteProductCommand, Response> {

        private readonly IProductService _productService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ProductCommandHandler(
            IProductService productService,
            ICurrentUserService currentUserService,
            IMapper mapper,
            IUnitOfWork unitOfWork) {
            _productService = productService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<AddProductResponse>> Handle(
            AddProductCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return Unauthorized<AddProductResponse>("User is not authenticated.");

            var product = new Product {
                Name = request.Name,
                Description = request.Description,
                PointsCost = request.PointsCost,
                Stock = request.Stock,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {
                var result = await _productService.CreateAsync(
                    product,
                    request.Images,
                    userId.Value,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null)
                    return BadRequest<AddProductResponse>(result.ErrorMessage);

                var createdProduct = await _productService.GetByIdAsync(result.Data.Id, cancellationToken);
                if (createdProduct is null)
                    return BadRequest<AddProductResponse>("Failed to retrieve created product.");

                var response = _mapper.Map<AddProductResponse>(createdProduct);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);
                return Created(response);
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest<AddProductResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<UpdateProductResponse>> Handle(
            UpdateProductCommand request,
            CancellationToken cancellationToken) {

            var userId = _currentUserService.UserId;
            if (!userId.HasValue)
                return Unauthorized<UpdateProductResponse>("User is not authenticated.");

            var existingProduct = await _productService.GetByIdAsync(request.Id, cancellationToken);
            if (existingProduct is null)
                return NotFound<UpdateProductResponse>("Product not found.");

            var product = new Product {
                Id = request.Id,
                Name = request.Name ?? existingProduct.Name,
                Description = request.Description ?? existingProduct.Description,
                PointsCost = request.PointsCost ?? existingProduct.PointsCost,
                Stock = request.Stock ?? existingProduct.Stock,
                IsActive = request.IsActive ?? existingProduct.IsActive
            };

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {
                var result = await _productService.UpdateAsync(
                    product,
                    request.Images,
                    userId.Value,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null) {
                    if (result.Status == ServiceOperationStatus.NotFound)
                        return NotFound<UpdateProductResponse>(result.ErrorMessage ?? "Product not found.");

                    return BadRequest<UpdateProductResponse>(result.ErrorMessage);
                }

                var updatedProduct = await _productService.GetByIdAsync(result.Data.Id, cancellationToken);
                if (updatedProduct is null)
                    return BadRequest<UpdateProductResponse>("Failed to retrieve updated product.");

                var response = _mapper.Map<UpdateProductResponse>(updatedProduct);

                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);
                return Success(response);
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest<UpdateProductResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response> Handle(
            DeleteProductCommand request,
            CancellationToken cancellationToken) {

            await using var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try {
                var productToDelete = await _productService.GetByIdAsync(request.Id, cancellationToken);
                if (productToDelete is null)
                    return NotFound("Product not found.");

                var result = await _productService.DeleteAsync(request.Id, cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null) {
                    if (result.Status == ServiceOperationStatus.NotFound)
                        return NotFound(result.ErrorMessage);

                    return BadRequest(result.ErrorMessage);
                }



                await _unitOfWork.SaveChangesAsync();
                await transaction.CommitAsync(cancellationToken);
                return Success();
            }
            catch (Exception ex) {
                await transaction.RollbackAsync(cancellationToken);
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}



