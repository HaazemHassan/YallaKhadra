using AutoMapper;
using MediatR;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.WasteReports.Commands.RequestModels;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Commands.Handlers {
    public class WasteReportCommandHandler : ResponseHandler,
                                              IRequestHandler<CreateWasteReportCommand, Response<WasteReportResponse>> {

        private readonly IWasteReportService _wasteReportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public WasteReportCommandHandler(
            IWasteReportService wasteReportService,
            ICurrentUserService currentUserService,
            IMapper mapper) {
            _wasteReportService = wasteReportService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<WasteReportResponse>> Handle(
            CreateWasteReportCommand request,
            CancellationToken cancellationToken) {
            try {
                // Get current user ID
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<WasteReportResponse>("User is not authenticated.");

                // Map command to WasteReport entity
                var wasteReport = _mapper.Map<WasteReport>(request);
                wasteReport.UserId = userId.Value;

                // Create waste report
                var result = await _wasteReportService.CreateAsync(
                    wasteReport,
                    request.Images,
                    cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null)
                    return BadRequest<WasteReportResponse>(result.ErrorMessage ?? "Failed to create waste report.");

                // Get the created report with related data for response
                var createdReport = await _wasteReportService.GetByIdAsync(result.Data.Id, cancellationToken);

                // Map to response DTO
                var response = _mapper.Map<WasteReportResponse>(createdReport);

                return Created(response);
            }
            catch (Exception ex) {
                return BadRequest<WasteReportResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
