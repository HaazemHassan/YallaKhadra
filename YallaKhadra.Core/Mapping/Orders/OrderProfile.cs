using AutoMapper;
using YallaKhadra.Core.Entities.E_CommerceEntities;
using YallaKhadra.Core.Features.Orders.Commands.Responses;
using YallaKhadra.Core.Features.Orders.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Orders {
    public class OrderProfile : Profile {
        public OrderProfile() {
            CreateMap<Order, GetMyOrdersResponse>()
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ItemsCount,
                    opt => opt.MapFrom(src => src.OrderItems.Count));

            CreateMap<Order, PlaceOrderResponse>()
                .ForMember(dest => dest.OrderId,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.ItemsCount,
                    opt => opt.MapFrom(src => src.GetItemsCount()));

            CreateMap<Order, GetOrderDetailsResponse>()
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductDescription,
                    opt => opt.MapFrom(src => src.Product.Description))
                .ForMember(dest => dest.ProductMainImageUrl,
                    opt => opt.MapFrom(src => src.Product.Images.Where(i => i.IsMain).Select(i => i.Url).FirstOrDefault()))
                .ForMember(dest => dest.TotalPoints,
                    opt => opt.MapFrom(src => src.CalculateTotalPoints()));

            CreateMap<OrderShippingDetails, ShippingDetailsDto>();
        }
    }
}
