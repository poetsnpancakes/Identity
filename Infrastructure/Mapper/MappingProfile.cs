using AutoMapper;
using Identity_Infrastructure.Context.SeedData;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Models.Authentication.SignUp;
using Identity_Infrastructure.Models.ResponseModel;
using Microsoft.AspNetCore.Identity;
namespace Identity_Infrastructure.Mapper
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
            .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.UserName))
             .ForMember(dest => dest.Gender, act => act.MapFrom(src => src.Gender));

            CreateMap<ApplicationUser, UserResponse>()
           .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email))
           .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.UserName));
           








        }
    }
}
