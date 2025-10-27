using AutoMapper;

namespace YallaKhadra.Core.Mapping.Users;

public partial class UserProfile : Profile {
    public UserProfile() {
        //AddUserMapping();
        GetUsersPaginatedMapping();
        GetUserByIdMapping();
        GetUserByUsernameMapping();
        SearchUsersMapping();
        //UpdateUserMapping();
    }
}
