using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Features.AIWasteScans.Queries.Models {
    public class GetAllAIWasteScansQuery : IRequest<PaginatedResult<AIWasteScanResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
