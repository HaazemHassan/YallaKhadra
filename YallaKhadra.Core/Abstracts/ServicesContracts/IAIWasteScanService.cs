using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface IAIWasteScanService {
        Task<ServiceOperationResult<AIWasteScan>> CreateScanAsync(IFormFile image,int userId,CancellationToken cancellationToken = default);
    }
}
