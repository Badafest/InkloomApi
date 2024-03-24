namespace InkloomApi.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>();
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateUserRequest, User>();
    }
}