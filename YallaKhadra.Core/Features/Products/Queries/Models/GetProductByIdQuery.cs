using MediatR;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Products.Queries.Responses;

namespace YallaKhadra.Core.Features.Products.Queries.Models {
    public class GetProductByIdQuery : IRequest<Response<GetProductByIdResponse>> {
        public int Id { get; set; }
    }
}
