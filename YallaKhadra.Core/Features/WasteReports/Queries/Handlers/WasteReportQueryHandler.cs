using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.WasteReports.Queries.Models;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Features.WasteReports.Queries.Handlers {
    public class WasteReportQueryHandler : ResponseHandler,
                                            IRequestHandler<GetWasteReportByIdQuery, Response<WasteReportResponse>> {

        private readonly IWasteReportRepository _wasteReportRepository;
        private readonly IMapper _mapper;

        public WasteReportQueryHandler(
            IWasteReportRepository wasteReportRepository,
            IMapper mapper) {
            _wasteReportRepository = wasteReportRepository;
            _mapper = mapper;
        }

        public async Task<Response<WasteReportResponse>> Handle(
            GetWasteReportByIdQuery request,
            CancellationToken cancellationToken) {
            try {
                // Get waste report by ID with related data
                var wasteReport = await _wasteReportRepository
                    .GetTableNoTracking(r => r.Id == request.Id)
                    .Include(r => r.Images)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(cancellationToken);

                if (wasteReport is null)
                    return NotFound<WasteReportResponse>($"Waste report with ID {request.Id} not found.");

                // Map to response DTO
                var response = _mapper.Map<WasteReportResponse>(wasteReport);

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<WasteReportResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
