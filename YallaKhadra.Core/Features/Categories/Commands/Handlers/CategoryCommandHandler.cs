using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Categories.Commands.RequestModels;
using YallaKhadra.Core.Features.Categories.Commands.Responses;

namespace YallaKhadra.Core.Features.Categories.Commands.Handlers {
    public class CategoryCommandHandler : ResponseHandler,
        IRequestHandler<AddCategoryCommand, Response<AddCategoryResponse>>,
        IRequestHandler<UpdateCategoryCommand, Response<UpdateCategoryResponse>>,
        IRequestHandler<DeleteCategoryCommand, Response> {

        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryCommandHandler(
            ICategoryService categoryService,
            IMapper mapper) {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<Response<AddCategoryResponse>> Handle(
            AddCategoryCommand request,
            CancellationToken cancellationToken) {
            try {
                var category = _mapper.Map<Category>(request);
                var result = await _categoryService.CreateAsync(category, cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null) {
                    return result.Status switch {
                        ServiceOperationStatus.AlreadyExists => Conflict<AddCategoryResponse>(result.ErrorMessage),
                        _ => BadRequest<AddCategoryResponse>(result.ErrorMessage),
                    };
                }

                var response = _mapper.Map<AddCategoryResponse>(result.Data);
                return Created(response);
            }
            catch (Exception ex) {
                return BadRequest<AddCategoryResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<UpdateCategoryResponse>> Handle(
            UpdateCategoryCommand request,
            CancellationToken cancellationToken) {
            try {
                var result = await _categoryService.UpdateAsync(
                    request.Id,
                    request.Name,
                    request.Description,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null) {
                    return result.Status switch {
                        ServiceOperationStatus.NotFound => NotFound<UpdateCategoryResponse>(result.ErrorMessage),
                        ServiceOperationStatus.AlreadyExists => Conflict<UpdateCategoryResponse>(result.ErrorMessage),
                        _ => BadRequest<UpdateCategoryResponse>(result.ErrorMessage),
                    };
                }

                var response = _mapper.Map<UpdateCategoryResponse>(result.Data);
                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<UpdateCategoryResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response> Handle(
            DeleteCategoryCommand request,
            CancellationToken cancellationToken) {
            try {
                var result = await _categoryService.DeleteAsync(request.Id, cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded) {
                    return result.Status switch {
                        ServiceOperationStatus.NotFound => NotFound(result.ErrorMessage),
                        _ => BadRequest(result.ErrorMessage),
                    };
                }

                return Success("Category deleted successfully.");
            }
            catch (Exception ex) {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
    }
}
