using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.IComponentModels;
using Fun.Domain.Fun.Models;
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

            CreateMap<IInitiativeComponentModel, Initiative>(MemberList.Source)
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ConstructUsing(sours => new Initiative {
                    Title = sours.Title,
                    Description = sours.Description,
                    CategoryId = sours.CategoryId
                });

            CreateMap<Initiative, IInitiativeComponentModel>()
                .ConstructUsingServiceLocator();


            CreateMap<ICategoryComponentModel, Category>()
                .ForMember(c => c.CategoryName, opt => opt.MapFrom(m => m.CategoryName));

            CreateMap<Category, ICategoryComponentModel>()
                .ConstructUsing(src => new CategoryComponentModel { CategoryName = src.CategoryName });

            CreateMap<ISubscribeComponentModel, Subscribe>()
               .ForMember(dest => dest.InitiativeId, opt => opt.MapFrom(src => src.InitiativeId))
               .ForMember(dest => dest.UserId, opt => opt.Ignore())
               .ForMember(dest => dest.Id, opt => opt.Ignore());
        }
    }
}
