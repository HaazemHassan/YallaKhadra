using MediatR;
using System.Text.Json.Serialization;
using YallaKhadra.Core.Bases.Responses;
using YallaKhadra.Core.Features.Carts.Commands.Responses;

namespace YallaKhadra.Core.Features.Carts.Commands.RequestModels {
    public class ToggleCartItemSelectionCommand : IRequest<Response<ToggleCartItemSelectionResponse>> {
        [JsonIgnore]
        public int CartItemId { get; set; }
        public bool IsSelected { get; set; }
    }
}
