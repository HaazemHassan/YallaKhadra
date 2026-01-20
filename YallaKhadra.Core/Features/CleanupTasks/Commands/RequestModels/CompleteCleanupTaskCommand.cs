using MediatR;
using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels {
    public class CompleteCleanupTaskCommand : IRequest<Response<string>> {
        public int TaskId { get; set; }
        public decimal FinalWeightInKg { get; set; }
        public int FinalWasteType { get; set; }
        public List<IFormFile>? Images { get; set; }
    }
}
