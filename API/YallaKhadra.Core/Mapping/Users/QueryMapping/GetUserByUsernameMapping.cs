using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Users {
    public partial class UserProfile {
        public void GetUserByUsernameMapping() {
            CreateMap<ApplicationUser, GetUserByUsernameResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}