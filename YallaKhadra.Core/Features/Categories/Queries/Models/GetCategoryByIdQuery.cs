using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Categories.Queries.Responses;

namespace YallaKhadra.Core.Features.Categories.Queries.Models {
    public class GetCategoryByIdQuery : IRequest<Response<GetCategoryByIdResponse>> {
        public int Id { get; set; }
    }
}
