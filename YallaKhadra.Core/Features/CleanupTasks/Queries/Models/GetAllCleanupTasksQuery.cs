using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Features.CleanupTasks.Queries.Models {
    public class GetAllCleanupTasksQuery : IRequest<PaginatedResult<CleanupTaskResponse>> {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
