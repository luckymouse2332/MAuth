using Mapster;
using MAuth.Data.Entities;
using MAuth.WebAPI.Commons.Helpers;
using MAuth.WebAPI.Controllers.Users.DTOs;
using MAuth.WebAPI.Controllers.Users.Requests;

namespace MAuth.WebAPI.Controllers.Users;

public class UserAdaptRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<User, UserDto>()
            .Map(dest => dest.Role, src => src.Role.ToString())
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.ForType<UserCreateRequest, User>()
            .Map(dest => dest.Role, src => EnumHelper.ParseEnum<UserRole>(src.Role));

        config.ForType<UserUpdateRequest, User>()
            .Map(dest => dest.Role, src => EnumHelper.ParseEnum<UserRole>(src.Role))
            .Map(dest => dest.Status, src => EnumHelper.ParseEnum<UserStatus>(src.Status));
    }
}
