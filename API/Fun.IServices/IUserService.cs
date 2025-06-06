using Fun.Application.ComponentModels;
using Fun.Application.IComponentModels;
using Fun.Application.ResponseModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Application.Fun.IServices
{
    public interface IUserService
    {
        public Task<object> Register(IUserRegisterModel userRegisterModel);
        public Task<IActionResult> AuthenticateUser(IUserAuthenticateModel authenticateUser);
        public Task<User> GetUser(string email);
        public Task<int> GetMy();
        Task<UserProfileResponseModel> GetMyProfileAsync();
        Task<UserProfileResponseModel> GetMyProfileAsyncBase64();
        AuthenticationProperties GetAuthenticationProperties(string returnUrl);
        Task<User> HandleCallbackAsync();
        Task<UserProfileResponseModel> UpdateProfileAsync(UserProfileComponentModel model);
        Task<string> SaveAvatarAsync(IFormFile file);
    }
}
