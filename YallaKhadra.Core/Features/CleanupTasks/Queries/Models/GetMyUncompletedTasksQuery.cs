using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Queries.Models {
    public class GetMyUncompletedTasksQuery : IRequest<Response<List<CleanupTaskResponse>>> {
    }
}
