using YallaKhadra.Core.Entities.BaseEntities;

namespace YallaKhadra.Core.Entities.E_CommerceEntities {
    public class OrderShippingDetails : BaseEntity<int> {
        public int OrderId { get; set; }
        public virtual Order Order { get; set; } = null!;

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string StreetAddress { get; set; } = string.Empty;
        public string? BuildingNumber { get; set; }
        public string? Landmark { get; set; }
        public string? ShippingNotes { get; set; }
    }
}