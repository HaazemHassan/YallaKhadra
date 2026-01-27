using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities.E_CommerceEntities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IOrderService {
        Task<ServiceOperationResult<Order>> PlaceOrderAsync(
            int userId,
            string fullName,
            string phoneNumber,
            string city,
            string streetAddress,
            string? buildingNumber,
            string? landmark,
            string? shippingNotes,
            CancellationToken cancellationToken = default);

        Task<ServiceOperationResult> CancelOrderAsync(
            int orderId,
            int userId,
            CancellationToken cancellationToken = default);
    }
}
