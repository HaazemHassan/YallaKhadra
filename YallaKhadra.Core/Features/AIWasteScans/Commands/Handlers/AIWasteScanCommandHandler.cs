using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Abstracts.ApiAbstracts;
using YallaKhadra.Core.Abstracts.InfrastructureAbstracts;
using YallaKhadra.Core.Abstracts.ServicesContracts;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.AIWasteScans.Commands.RequestModels;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Features.AIWasteScans.Commands.Handlers {
    public class AIWasteScanCommandHandler : ResponseHandler,
                                              IRequestHandler<CreateAIWasteScanCommand, Response<AIWasteScanResponse>> {

        private readonly IAIWasteScanRepository _scanRepository;
        private readonly IAIWasteScanService _scanService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public AIWasteScanCommandHandler(IAIWasteScanRepository scanRepository,IAIWasteScanService scanService,ICurrentUserService currentUserService,IMapper mapper) {
            _scanRepository = scanRepository;
            _scanService = scanService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<Response<AIWasteScanResponse>> Handle(CreateAIWasteScanCommand request,CancellationToken cancellationToken) {
            try {
                var userId = _currentUserService.UserId;
                if (!userId.HasValue)
                    return Unauthorized<AIWasteScanResponse>("User is not authenticated.");

                var result = await _scanService.CreateScanAsync(request.Image,userId.Value,cancellationToken);

                if (result.Status != ServiceOperationStatus.Succeeded || result.Data is null)
                    return BadRequest<AIWasteScanResponse>(result.ErrorMessage ?? "Failed to create AI waste scan.");

                var createdScan = await _scanRepository
                    .GetTableNoTracking(s => s.Id == result.Data.Id)
                    .Include(s => s.WasteScanImage)
                    .Include(s => s.User)
                    .FirstOrDefaultAsync(cancellationToken);

                var response = _mapper.Map<AIWasteScanResponse>(createdScan);

                return Created(response);
            }
            catch (Exception ex) {
                return BadRequest<AIWasteScanResponse>($"An error occurred: {ex.Message}");
            }
        }
    }
}
