using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Categories.Queries.Models;
using YallaKhadra.Core.Features.Categories.Queries.Responses;

namespace YallaKhadra.Core.Features.Categories.Queries.Handlers {
    public class CategoryQueryHandler : ResponseHandler,
                                        IRequestHandler<GetCategoryByIdQuery, Response<GetCategoryByIdResponse>>,
                                        IRequestHandler<GetCategoryByNameQuery, Response<GetCategoryByNameResponse>> {

        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryQueryHandler(
            ICategoryRepository categoryRepository,
            IMapper mapper) {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<Response<GetCategoryByIdResponse>> Handle(
            GetCategoryByIdQuery request,
            CancellationToken cancellationToken) {
            try {
                var category = await _categoryRepository
                    .GetTableNoTracking(c => c.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (category is null)
                    return NotFound<GetCategoryByIdResponse>("Category not found.");

                var response = _mapper.Map<GetCategoryByIdResponse>(category);
                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<GetCategoryByIdResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<GetCategoryByNameResponse>> Handle(
            GetCategoryByNameQuery request,
            CancellationToken cancellationToken) {
            try {
                var category = await _categoryRepository
                    .GetTableNoTracking(c => c.Name == request.Name)
                    .FirstOrDefaultAsync(cancellationToken);

                if (category is null)
                    return NotFound<GetCategoryByNameResponse>("Category not found.");

                var response = _mapper.Map<GetCategoryByNameResponse>(category);
                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<GetCategoryByNameResponse>($"An error occurred: {ex.Message}");
            }
        }


    }
}
