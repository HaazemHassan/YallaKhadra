using AutoMapper;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Features.Categories.Commands.RequestModels;
using YallaKhadra.Core.Features.Categories.Commands.Responses;
using YallaKhadra.Core.Features.Categories.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Categories {
    public class CategoryProfile : Profile {
        public CategoryProfile() {
            CreateMap<Category, GetCategoryByIdResponse>();
            CreateMap<Category, GetCategoryByNameResponse>();

            CreateMap<Product, ProductInCategoryDto>()
                .ForMember(
                    dest => dest.MainImageUrl,
                    opt => opt.MapFrom(src =>
                        src.Images
                            .Where(i => i.IsMain)
                            .Select(i => i.Url)
                            .FirstOrDefault()
                    )
                );

            CreateMap<AddCategoryCommand, Category>();
            CreateMap<Category, AddCategoryResponse>();
            CreateMap<Category, UpdateCategoryResponse>();
        }
    }
}
