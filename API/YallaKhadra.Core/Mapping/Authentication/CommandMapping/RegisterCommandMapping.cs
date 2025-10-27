using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Authentication.Commands.RequestsModels;

namespace YallaKhadra.Core.Mapping.Authentication {
    public partial class AuthenticationProfile {
        public void RegisterMapping() {
            CreateMap<RegisterCommand, ApplicationUser>();

        }
    }
}
