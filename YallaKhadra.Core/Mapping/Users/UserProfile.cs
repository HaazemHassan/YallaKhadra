using AutoMapper;

namespace YallaKhadra.Core.Mapping.Users;

public partial class UserProfile : Profile {
    public UserProfile() {
        RegisterMapping();
        AddUserMapping();
        GetUsersPaginatedMapping();
        GetUserByIdMapping();
    }
}
