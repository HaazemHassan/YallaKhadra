using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Commands.Handlers {
public class CleanupTaskCommandHandler : ResponseHandler,
                                          IRequestHandler<AssignCleanupTaskCommand, Response<string>>,
                                          IRequestHandler<CompleteCleanupTaskCommand, Response<string>> 
    {
        private readonly ICleanupTaskService _cleanupTaskService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public CleanupTaskCommandHandler(ICleanupTaskService cleanupTaskService,ICurrentUserService currentUserService,IMapper mapper,IUnitOfWork unitOfWork) {
            _cleanupTaskService = cleanupTaskService;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<Response<string>> Handle(AssignCleanupTaskCommand request,CancellationToken cancellationToken) {
            await _unitOfWork.BeginTransactionAsync();
            try {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<string>("User is not authenticated.");


                var result = await _cleanupTaskService.AssignReportAsync(request.ReportId,userId.Value, cancellationToken);


                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null)
                    return BadRequest<string>(result.ErrorMessage ?? "Failed to assign cleanup task.");


                await _unitOfWork.CommitAsync();
                return Created("Task assigned successfully");
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<string>> Handle(CompleteCleanupTaskCommand request,CancellationToken cancellationToken) {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<string>("User is not authenticated.");


                var result = await _cleanupTaskService.CompleteTaskAsync(request.TaskId,userId.Value,request.FinalWeightInKg,request.FinalWasteType,request.Images,cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null)
                    return BadRequest<string>(result.ErrorMessage ?? "Failed to complete cleanup task.");

                await _unitOfWork.CommitAsync();
                return Success(message: "Cleanup task completed successfully.");
            }
            catch (Exception ex) {
                await _unitOfWork.RollbackAsync();
                return BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
    }
}
