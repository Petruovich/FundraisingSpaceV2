using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Urb.Infrastructure.Fun.Services
{
    public class UserService: IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private MainDataContext _context;
        private ITokenService _jwtService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _redirectUri;
        //private readonly IUserAuthenticateModel _userAuthenticateModel;

        public UserService(
            IHttpContextAccessor httpContextAccessor,
            MainDataContext context,
            ITokenService jWTService,
            IMapper autoMapperProfile,
            SignInManager<User> signInManager,
            IConfiguration configuration,
           // IUserAuthenticateModel userAuthenticateModel,
            UserManager<User> userManager

            )
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            //_userAuthenticateModel = userAuthenticateModel;
            _jwtService = jWTService;
            _mapper = autoMapperProfile;
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _redirectUri = _configuration["AppSettings:ClientUrl"]
                         + "/api/account/externalLogincallback";
        }
        public IActionResult Ok(User user)
        {
            throw new NotImplementedException();
        }
        public async Task<User> GetUser(string email)
        {
            var identityuser = await _userManager.FindByEmailAsync(email);
            return identityuser;
        }
        public async Task<object> Register(IUserRegisterModel userRegisterModel)
        {
            var user = _mapper.Map<User>(userRegisterModel);
            var testEmail = await GetUser(user.Email);
            if (testEmail != null)
            {
                return new { Error = "Email already exist" };
            }
            var result = await _userManager.CreateAsync(user, userRegisterModel.Password);

            if (result.Succeeded)
            {
                var authUser = _mapper.Map<UserAuthenticateModel>(userRegisterModel);
                _context.Users.Add(user);
                return result;
            }
            List<IdentityError> errors = result.Errors.ToList();
            var errorDetail = string.Join(", ", errors.Select(e => e.Description));
            return new { Error = errorDetail };
        }
        public async Task<IActionResult> AuthenticateUser(IUserAuthenticateModel authenticateUser)
        {
            var user = _mapper.Map<User>(authenticateUser);
            var joinUser = await _userManager.FindByEmailAsync(user.Email);
            if (joinUser == null)
            {
                return new BadRequestObjectResult(new { Message = "User not found" });
            }
            var userauth = await _signInManager.CheckPasswordSignInAsync(joinUser, authenticateUser.Password,
                                                                           lockoutOnFailure: false);
            if (userauth.Succeeded)
            {
                var token = _jwtService.GenerateToken(authenticateUser);
                var isAdmin = authenticateUser.Email;
                string supportadmin = "alexsemenov@gmaill.com";
                bool IsSupportAdmin = string.Equals(isAdmin, supportadmin, StringComparison.OrdinalIgnoreCase);;
                return new OkObjectResult(new
                {
                    AuthToken = token,
                    IsAdminSupport = IsSupportAdmin
                }
                );            
            }
            return new BadRequestObjectResult(new { Message = "Wrong password" });
        }
        [Authorize]
        public async Task<int> GetMy()
        {
            var userEmail = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);
            var result = await _userManager.FindByEmailAsync(userEmail);
            var id = result.Id;
            return id;
        }

        public AuthenticationProperties GetAuthenticationProperties(string returnUrl)
        {
            //var properties = _signInManager.ConfigureExternalAuthenticationProperties(
            //    GoogleDefaults.AuthenticationScheme,
            //    $"{_redirectUri}?returnUrl={Uri.EscapeDataString(returnUrl)}");
            var properties =  _signInManager.ConfigureExternalAuthenticationProperties(
  GoogleDefaults.AuthenticationScheme,/*, returnUrl*/
  "/api/User/ExternalLoginCallback");
            return properties;
        }

        public async Task<User> HandleCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
                throw new InvalidOperationException("Cannot load external login information.");
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false);

            User user;
            if (result.Succeeded)
            {
                user = await _userManager.FindByLoginAsync(
                    info.LoginProvider, info.ProviderKey);
            }
            else
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                user = new User { UserName = email, Email = email };
                await _userManager.CreateAsync(user);
                await _userManager.AddLoginAsync(user, info);
            }
            return user;
        }
    }
}
