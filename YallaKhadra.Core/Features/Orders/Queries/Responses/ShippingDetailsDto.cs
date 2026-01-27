namespace YallaKhadra.Core.Features.Orders.Queries.Responses {
    public class ShippingDetailsDto {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string? BuildingNumber { get; set; }
        public string? Landmark { get; set; }
        public string? ShippingNotes { get; set; }
    }
}
