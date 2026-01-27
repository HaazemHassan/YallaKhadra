using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Orders.Commands.Responses;

namespace YallaKhadra.Core.Features.Orders.Commands.RequestModels {
    public class PlaceOrderCommand : IRequest<Response<PlaceOrderResponse>> {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string? BuildingNumber { get; set; }
        public string? Landmark { get; set; }
        public string? ShippingNotes { get; set; }
    }
}
