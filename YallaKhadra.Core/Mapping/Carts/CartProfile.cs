using AutoMapper;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Features.Carts.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Carts {
    public class CartProfile : Profile {
        public CartProfile() {
            CreateMap<Cart, GetCartResponse>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src =>
                    src.Items.Where(i => i.IsSelected).Sum(i => i.PointsCost * i.Quantity)))
                .ForMember(dest => dest.TotalPointsWithCurrentPrices, opt => opt.MapFrom(src =>
                    src.Items.Where(i => i.IsSelected).Sum(i => i.Product.PointsCost * i.Quantity)))
                .ForMember(dest => dest.HasPriceChanges, opt => opt.MapFrom(src =>
                    src.Items.Any(i => i.PointsCost != i.Product.PointsCost)));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.ProductMainImageUrl, opt => opt.MapFrom(src =>
                    src.Product.Images.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault()))
                .ForMember(dest => dest.CurrentProductPointsCost, opt => opt.MapFrom(src => src.Product.PointsCost))
                .ForMember(dest => dest.PriceChanged, opt => opt.MapFrom(src => src.PointsCost != src.Product.PointsCost))
                .ForMember(dest => dest.TotalPoints, opt => opt.MapFrom(src => src.PointsCost * src.Quantity))
                .ForMember(dest => dest.TotalPointsWithCurrentPrice, opt => opt.MapFrom(src => src.Product.PointsCost * src.Quantity));
        }
    }
}
