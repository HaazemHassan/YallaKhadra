using YallaKhadra.Core.Entities.IdentityEntities;
using YallaKhadra.Core.Features.Users.Commands.RequestModels;

namespace YallaKhadra.Core.Mapping.Users {
    public partial class UserProfile {
        public void UpdateUserMapping() {
            CreateMap<UpdateUserCommand, ApplicationUser>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImage, opt => opt.Ignore())
                .ForMember(dest => dest.ProfileImageId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember)
                 => !string.IsNullOrWhiteSpace(srcMember as string)));
        }
    }
}
