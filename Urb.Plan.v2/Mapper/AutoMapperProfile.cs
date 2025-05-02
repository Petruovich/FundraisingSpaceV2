using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.IComponentModels;
using Microsoft.AspNetCore.Identity;
using Urb.Domain.Urb.Models;
using Urb.Plan.v2.Views;

namespace Urb.Plan.v2.Mapper
{
    public class AutoMapperProfile: Profile 
    {
        public AutoMapperProfile()
        {
            CreateMap<IUserRegisterModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.FirstName}.{src.SecondName}"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ConstructUsing(sours => new User { });

            CreateMap<IUserRegisterModel, IUserAuthenticateModel>()
                .ForMember(aut => aut.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(aut => aut.Password, opt => opt.MapFrom(src => src.Password))
                .ConstructUsing(sours => new UserAuthenticateModel { });

            CreateMap<IUserAuthenticateModel, User>()
                .ForMember(aut => aut.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(aut => aut.PasswordHash, opt => opt.MapFrom(src => src.Password))
                .ConstructUsing(sours => new User { });
        }
    }
}
