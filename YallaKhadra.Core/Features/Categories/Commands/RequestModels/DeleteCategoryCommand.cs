using MediatR;
using YallaKhadra.Core.Bases.Responses;

namespace YallaKhadra.Core.Features.Categories.Commands.RequestModels {
    public class DeleteCategoryCommand : IRequest<Response> {
        public int Id { get; set; }
    }
}
