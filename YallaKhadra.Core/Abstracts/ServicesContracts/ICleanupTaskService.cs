using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts {
    public interface ICleanupTaskService {

        Task<ServiceOperationResult<CleanupTask>> AssignReportAsync(int reportId,int workerId,CancellationToken cancellationToken = default);

        Task<ServiceOperationResult<CleanupTask>> CompleteTaskAsync(int taskId, int workerId,decimal finalWeightInKg,int finalWasteType,List<IFormFile>? images,CancellationToken cancellationToken = default);

        Task<List<CleanupTask>> GetWorkerUncompletedTasksAsync(int workerId, CancellationToken cancellationToken = default);
    }
}
