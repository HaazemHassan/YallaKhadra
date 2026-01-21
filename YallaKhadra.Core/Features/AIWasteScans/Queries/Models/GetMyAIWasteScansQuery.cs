using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Features.AIWasteScans.Queries.Models {
    public class GetMyAIWasteScansQuery : IRequest<Response<List<AIWasteScanResponse>>> {
    }
}
