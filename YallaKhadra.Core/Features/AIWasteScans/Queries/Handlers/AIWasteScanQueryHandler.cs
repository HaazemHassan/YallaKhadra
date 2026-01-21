using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Models;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Features.AIWasteScans.Queries.Handlers {
    public class AIWasteScanQueryHandler : ResponseHandler,
                                            IRequestHandler<GetAIWasteScanByIdQuery, Response<AIWasteScanResponse>>,
                                            IRequestHandler<GetMyAIWasteScansQuery, Response<List<AIWasteScanResponse>>>,
                                            IRequestHandler<GetAllAIWasteScansQuery, PaginatedResult<AIWasteScanResponse>> {

        private readonly IAIWasteScanRepository _scanRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AIWasteScanQueryHandler(IAIWasteScanRepository scanRepository,ICurrentUserService currentUserService,IMapper mapper) {
            _scanRepository = scanRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<AIWasteScanResponse>> Handle(GetAIWasteScanByIdQuery request,CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<AIWasteScanResponse>("User is not authenticated.");

                var scan = await _scanRepository
                    .GetTableNoTracking(s => s.Id == request.Id)
                    .Include(s => s.WasteScanImage)
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(cancellationToken);

                if (scan is null)
                    return NotFound<AIWasteScanResponse>($"AI Waste Scan with ID {request.Id} not found.");

                var response = _mapper.Map<AIWasteScanResponse>(scan);

                return Success(response);
            }
            catch (Exception ex) {
                return BadRequest<AIWasteScanResponse>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<Response<List<AIWasteScanResponse>>> Handle(GetMyAIWasteScansQuery request,CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<List<AIWasteScanResponse>>("User is not authenticated.");

                var scans = await _scanRepository
                    .GetTableNoTracking(s => s.UserId == userId.Value)
                    .Include(s => s.WasteScanImage)
                    .OrderByDescending(s => s.CreatedAt)
                    .ToListAsync(cancellationToken);

                var response = _mapper.Map<List<AIWasteScanResponse>>(scans);

                return Success(response, message: $"Retrieved {response.Count} scans.");
            }
            catch (Exception ex) {
                return BadRequest<List<AIWasteScanResponse>>($"An error occurred: {ex.Message}");
            }
        }

        public async Task<PaginatedResult<AIWasteScanResponse>> Handle(GetAllAIWasteScansQuery request,CancellationToken cancellationToken) {
            try {
                var scansQuery = _scanRepository
                    .GetTableNoTracking()
                    .Include(s => s.WasteScanImage)
                    .Include(s => s.User)
                    .OrderByDescending(s => s.CreatedAt);

                var paginatedResult = await scansQuery
                    .ProjectTo<AIWasteScanResponse>(_mapper.ConfigurationProvider)
                    .ToPaginatedResultAsync(request.PageNumber, request.PageSize);

                return paginatedResult;
            }
            catch (Exception ex) {
                return PaginatedResult<AIWasteScanResponse>.Failure($"An error occurred: {ex.Message}");
            }
        }
    }
}
