namespace Inkloom.Api.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserResponse>()
                .ForMember(dest => dest.Followers, opt => opt.MapFrom(src => src.Followers.Count))
                .ForMember(dest => dest.Following, opt => opt.MapFrom(src => src.Followings.Count))
                .ForMember(dest => dest.Blogs, opt => opt.MapFrom(src => src.Blogs.Count));
        CreateMap<RegisterRequest, User>();
        CreateMap<UpdateUserRequest, User>();
    }
}