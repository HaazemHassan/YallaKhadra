using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Users {
    public partial class UserProfile {
        public void GetUsersPaginatedMapping() {
            CreateMap<ApplicationUser, GetUsersPaginatedResponse>()
                .ForMember(dest => dest.Phone,
                   opt => opt.MapFrom(src => src.PhoneNumber));

        }
    }
}


