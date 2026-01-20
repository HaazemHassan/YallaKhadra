using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Helpers;

namespace YallaKhadra.Services.Services {
    public class CleanupTaskService : ICleanupTaskService {
        private readonly ICleanupTaskRepository _cleanupTaskRepository;
        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly IImageService<CleanupImage> _imageService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPointsTransactionService _pointsTransactionService;

        public CleanupTaskService(ICleanupTaskRepository cleanupTaskRepository,IWasteReportRepository wasteReportRepository,IImageService<CleanupImage> imageService,UserManager<ApplicationUser> userManager,IPointsTransactionService pointsTransactionService) 
        {
            _cleanupTaskRepository = cleanupTaskRepository;
            _wasteReportRepository = wasteReportRepository;
            _imageService = imageService;
            _userManager = userManager;
            _pointsTransactionService = pointsTransactionService;
        }

        public async Task<ServiceOperationResult<CleanupTask>> AssignReportAsync(int reportId,int workerId,CancellationToken cancellationToken = default) {
            try {
                // Get worker
                var worker = await _userManager.FindByIdAsync(workerId.ToString());
                if (worker is null)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.NotFound,"Worker not found.")!;

                // Verify worker role
                var roles = await _userManager.GetRolesAsync(worker);
                if (!roles.Contains(UserRole.Worker.ToString()))
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,"User is not a worker.")!;

                // Get report
                var report = await _wasteReportRepository
                    .GetTableAsTracking(r => r.Id == reportId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (report is null)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.NotFound,"Report not found.")!;

                // Check if report is already assigned
                if (report.Status != ReportStatus.Pending)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,$"Report is already {report.Status}. Only pending reports can be assigned.")!;


                var cleanupTask = new CleanupTask {
                    ReportId = reportId,
                    WorkerId = workerId
                };

                await _cleanupTaskRepository.AddAsync(cleanupTask);

                // Update report status
                report.Status = ReportStatus.InProgress;
                await _wasteReportRepository.UpdateAsync(report);

           

                return ServiceOperationResult<CleanupTask>.Success(cleanupTask)!;
            }
            catch (Exception ex) {
                return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,$"Failed to assign report: {ex.Message}")!;
            }
        }

        public async Task<ServiceOperationResult<CleanupTask>> CompleteTaskAsync(int taskId,int workerId,decimal finalWeightInKg,int finalWasteType,List<IFormFile>? images,CancellationToken cancellationToken = default) {
            try {
                if (!Enum.IsDefined(typeof(WasteType), finalWasteType))
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,"Invalid waste type.")!;

       
                if (finalWeightInKg <= 0)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed, "Weight must be greater than zero.")!;

                var task = await _cleanupTaskRepository.GetTableAsTracking(t => t.Id == taskId)
                    .Include(t => t.Report)
                    .FirstOrDefaultAsync(cancellationToken);

                if (task is null)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.NotFound,"Cleanup task not found.")!;

                if (task.WorkerId != workerId)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Unauthorized,"You are not assigned to this task.")!;

                if (task.CompletedAt.HasValue)
                    return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,"Task is already completed.")!;

                // Update task
                task.CompletedAt = DateTime.UtcNow;
                task.FinalWeightInKg = finalWeightInKg;
                task.FinalWasteType = (WasteType)finalWasteType;

                await _cleanupTaskRepository.UpdateAsync(task);

                // Update report status
                task.Report.Status = ReportStatus.Done;
                await _wasteReportRepository.UpdateAsync(task.Report);

                // Upload images if provided
                if (images != null && images.Count > 0) {
                    foreach (var image in images) {
                        if (image.Length > 0) {
                            await _imageService.UploadAsync(
                                image,
                                workerId,
                                task.Id,
                                cancellationToken);
                        }
                    }
                }

                int points = PointsWasteCalculatorHelper.CalculatePoints((WasteType)finalWasteType, (task.Report.WasteType == (WasteType)finalWasteType), finalWeightInKg);
                await _pointsTransactionService.EarnPointsAsync(task.Report.UserId, points, PointsSourceType.WasteReport, task.Report.Id);


                return ServiceOperationResult<CleanupTask>.Success(task)!;
            }
            catch (Exception ex) {
                return ServiceOperationResult<CleanupTask>.Failure(ServiceOperationStatus.Failed,$"Failed to complete task: {ex.Message}")!;
            }
        }

        public async Task<List<CleanupTask>> GetWorkerUncompletedTasksAsync(int workerId, CancellationToken cancellationToken = default) {
            return await _cleanupTaskRepository
                .GetTableNoTracking(t => t.WorkerId == workerId && t.CompletedAt == null)
                .Include(t => t.Report)
                    .ThenInclude(r => r.Images)
                .Include(t => t.Report)
                    .ThenInclude(r => r.User)
                .Include(t => t.Worker)
                .OrderByDescending(t => t.AssignedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
