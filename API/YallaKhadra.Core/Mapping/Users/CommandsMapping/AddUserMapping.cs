using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;
using YallaKhadra.Core.Features.Users.Commands.Responses;

namespace YallaKhadra.Core.Mapping.Users {
    public partial class UserProfile {
        public void AddUserMapping() {
            CreateMap<AddUserCommand, ApplicationUser>();
            CreateMap<ApplicationUser, AddUserResponse>();

        }
    }
}
