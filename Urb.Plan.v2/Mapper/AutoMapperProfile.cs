using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.IComponentModels;
using Fun.Application.ResponseModels;
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

            CreateMap<FundraisingComponentModel, Fundraising>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedById, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.Initiative, opt => opt.Ignore())
                .ForMember(dest => dest.Stat, opt => opt.Ignore())
                .ForMember(dest => dest.Donates, opt => opt.Ignore());

            //CreateMap<InitiativeComponentModel, Initiative>()
            //    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
            //    .ForMember(dest => dest.UserId, opt => opt.Ignore())
            //    .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
            //    .ForMember(dest => dest.Stat, opt => opt.Ignore())
            //    .ForMember(dest => dest.Fundraisings, opt => opt.Ignore())
            //    .ForMember(dest => dest.Subscribes, opt => opt.Ignore());

            CreateMap<Initiative, InitiativeResponseModel>()
                .ForMember(dest => dest.ImageBase64, opt => opt.Ignore())
                .ForMember(dest => dest.Title, opt => opt.MapFrom(s => s.Title))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(s => s.Description))
                //.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(s => s.ImageUrl))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(s => s.Id));

            CreateMap<User, UserProfileResponseModel>()
                // UserName у нас зберігається як "FirstName.SecondName". Розбиваємо по крапці:
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src =>
                !string.IsNullOrWhiteSpace(src.UserName)
                ? src.UserName.Split('.', StringSplitOptions.None)[0]
                : string.Empty
    ))
                .ForMember(dest => dest.SecondName, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.UserName) && src.UserName.Contains('.')
                        ? src.UserName.Split('.', StringSplitOptions.None).Skip(1).FirstOrDefault() ?? string.Empty
                        : string.Empty
                ))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.ImageBase64, opt => opt.Ignore())
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<UserProfileResponseModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    $"{src.FirstName}.{src.SecondName}"
                ))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))                
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))

                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.SecurityStamp, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnabled, opt => opt.Ignore())
                .ForMember(dest => dest.LockoutEnd, opt => opt.Ignore())
                .ForMember(dest => dest.AccessFailedCount, opt => opt.Ignore())
                .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedUserName, opt => opt.Ignore())
                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                .ForMember(dest => dest.NormalizedEmail, opt => opt.Ignore());

            CreateMap<UserProfileComponentModel, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom((src, dest) =>
                {
                    if (string.IsNullOrWhiteSpace(src.FirstName) && string.IsNullOrWhiteSpace(src.SecondName))
                        return dest.UserName;
                    var f = src.FirstName?.Trim() ?? string.Empty;
                    var s = src.SecondName?.Trim() ?? string.Empty;
                    return $"{f}.{s}";
                }))
                                .ForMember(dest => dest.Email, opt =>
                {
                    opt.MapFrom(src => src.Email);
                })
                                .ForMember(dest => dest.PhoneNumber, opt =>
                {
                    opt.MapFrom(src => src.PhoneNumber);
                })
                                .ForMember(dest => dest.AvatarUrl, opt => opt.Ignore())
                            .ForAllMembers(opt =>
                {
                    opt.Condition((src, dest, srcMember) =>
                    {
                        return srcMember != null;
                    });
                });
        }
    }
}
