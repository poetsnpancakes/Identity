using AutoMapper;
using Infrastructure.Context.SeedData;
using Infrastructure.Entity;
using Infrastructure.Models.Authentication.SignUp;
using Infrastructure.Models.ResponseModel;
using Microsoft.AspNetCore.Identity;
namespace Infrastructure.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            /*  CreateMap<EmployeeResponse, Employee>()
                  .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.UserName))
                  .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email));

              */
            CreateMap<UserRole, IdentityRole>()
                  .ForMember(dest => dest.Name, act => act.MapFrom(src => src.Name))
                  .ForMember(dest => dest.NormalizedName, act => act.MapFrom(src => src.NormalizedName))
                  .ForMember(dest => dest.ConcurrencyStamp, act => act.MapFrom(src => src.ConcurrencyStamp));


            CreateMap<UserRequest, ApplicationUser>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.UserName));

            CreateMap<ApplicationUser, UserResponse>()
           .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
           .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.UserName));








        }
    }
}
