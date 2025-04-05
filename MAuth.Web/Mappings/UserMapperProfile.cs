using AutoMapper;
using MAuth.Web.Helpers.Mapper;
using MAuth.Web.Models.DTOs;
using MAuth.Web.Models.Entities;

namespace MAuth.Web.Mappings
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<UserCreateDto, User>()
                .ForEnumMember(dest => dest.Role, src => src.Role);

            CreateMap<UserUpdateDto, User>()
                .ForEnumMember(dest => dest.Status, src => src.Status)
                .ForEnumMember(dest => dest.Role, src => src.Role);
        }
    }
}
