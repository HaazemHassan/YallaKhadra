using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Categories.Commands.Responses;

namespace YallaKhadra.Core.Features.Categories.Commands.RequestModels {
    public class AddCategoryCommand : IRequest<Response<AddCategoryResponse>> {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
