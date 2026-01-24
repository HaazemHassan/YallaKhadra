using MediatR;
using System.Text.Json.Serialization;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Categories.Commands.Responses;

namespace YallaKhadra.Core.Features.Categories.Commands.RequestModels {
    public class UpdateCategoryCommand : IRequest<Response<UpdateCategoryResponse>> {
        [JsonIgnore]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
