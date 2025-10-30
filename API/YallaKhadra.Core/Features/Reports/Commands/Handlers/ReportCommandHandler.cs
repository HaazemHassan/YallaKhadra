using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;

namespace YallaKhadra.Core.Features.Reports.Commands.Handlers
{
    public class ReportCommandHandler : ResponseHandler,
                                        IRequestHandler<AddReportCommand, Response<string>>,
                                        IRequestHandler<ReviewReportCommand, Response<string>>
    {
        #region Fields
        private readonly IReportService _reportService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        #endregion
        #region Constructors
        public ReportCommandHandler(
            IReportService reportService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IPhotoService photoService,
            IMapper mapper)
        {
            _reportService = reportService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _photoService = photoService;
            _mapper = mapper;
        }
        #endregion

        #region functions
        public async Task<Response<string>> Handle(AddReportCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                //maping 
                var report = _mapper.Map<Report>(request);
                //Add report
                var addReportResult = await _reportService.AddReportAsync(report, request.Photos);
                if (addReportResult.Status != ServiceOperationStatus.Succeeded || addReportResult.Data is null)
                {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<string>(addReportResult.ErrorMessage);
                }

                //Add photos
                var addPhotosResult = await _photoService.UploadRportPhotos(request.Photos, addReportResult.Data.Id);
                if (addPhotosResult.Status != ServiceOperationStatus.Succeeded || addPhotosResult.Data is null)
                {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<string>(addReportResult.ErrorMessage);
                }

                await _unitOfWork.CommitAsync();
                return Success<string>("Report added successfully"!);


            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest<string>($"An error occurred: {ex.Message}");

            }

        }

        public async Task<Response<string>> Handle(ReviewReportCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var reviewerId = _currentUserService.UserId;
                if (reviewerId == null)
                {
                    await _unitOfWork.RollbackAsync();
                    return Unauthorized<string>("User is not authenticated");
                }

                // Review the report
                var reviewResult = await _reportService.ReviewReportAsync(
                    request.ReportId,
                    request.IsApproved,
                    request.Notes,
                    reviewerId.Value);

                if (reviewResult.Status != ServiceOperationStatus.Succeeded || reviewResult.Data is null)
                {
                    await _unitOfWork.RollbackAsync();
                    return BadRequest<string>(reviewResult.ErrorMessage);
                }

                await _unitOfWork.CommitAsync();

                var statusMessage = request.IsApproved ? "approved and set to InProgress" : "rejected";
                return Success<string>($"Report has been {statusMessage} successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                return BadRequest<string>($"An error occurred: {ex.Message}");
            }
        }
        #endregion
    }
}
