using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Features.AIWasteScans.Commands.RequestModels {
    public class CreateAIWasteScanCommand : IRequest<Response<AIWasteScanResponse>> {
        public IFormFile Image { get; set; } = null!;
    }
}
