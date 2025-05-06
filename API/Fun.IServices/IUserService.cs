using Fun.Application.IComponentModels;
using Microsoft.AspNetCore.Authentication;
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
        public Task<string> GetMy();
        AuthenticationProperties GetAuthenticationProperties(string returnUrl);
        Task<User> HandleCallbackAsync();
    }
}
