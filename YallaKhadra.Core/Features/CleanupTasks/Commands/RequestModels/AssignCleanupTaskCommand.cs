using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Commands.RequestModels {
    public class AssignCleanupTaskCommand : IRequest<Response<string>> {
        public int ReportId { get; set; }
    }
}
