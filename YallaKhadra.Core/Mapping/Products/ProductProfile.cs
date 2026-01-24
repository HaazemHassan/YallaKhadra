using AutoMapper;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Features.Products.Commands.Responses;
using YallaKhadra.Core.Features.Products.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Products;

public class ProductProfile : Profile {
    public ProductProfile() {
        CreateMap<Product, AddProductResponse>();
        CreateMap<ProductImage, ProductImageDto>();
        CreateMap<Category, CategoryDto>();

        CreateMap<Product, GetProductByIdResponse>();
        CreateMap<Product, GetProductsPaginatedResponse>()
            .ForMember(
                     dest => dest.MainImageUrl,
                     opt => opt.MapFrom(src =>
                     src.Images
                    .Where(i => i.IsMain)
                    .Select(i => i.Url)
                    .FirstOrDefault()
            )
        );

        CreateMap<Product, UpdateProductResponse>();
    }
}
