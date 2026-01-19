using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IWasteReportService {
        Task<ServiceOperationResult<WasteReport>> CreateAsync(
            WasteReport wasteReport,
            List<IFormFile>? images,
            CancellationToken cancellationToken = default);

        Task<WasteReport?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
