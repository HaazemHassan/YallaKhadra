using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users;
using YallaKhadra.Core.Features.Users.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Users {
    public partial class UserProfile {
        public void GetUserByIdMapping() {
            CreateMap<ApplicationUser, GetUserByIdResponse>()
                .ForMember(dest => dest.ProfileImage,
                   opt => opt.MapFrom(src => src.ProfileImage));
            
            CreateMap<UserProfileImage, UserProfileImageDto>();
        }
    }
}