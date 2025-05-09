
using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Application.IControllers;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Urb.Application.App.Settings;
using Urb.Domain.Urb.Models;



namespace Urb.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class UserController : ControllerBase, IUserController
    {
        private IUserService _userService;
        private ITokenService _jwtService;
        private IMapper _mapper;
        private AppSettings _appSettings;
        private User _user;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly IGoogleAuthService _googleAuthService;
        public UserController(
           User user,
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor,
            ITokenService jwtService
            //IGoogleAuthService googleAuthService)
        {
            _jwtService = jwtService;
            _user = user;
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;            //_googleAuthService = googleAuthService;
        }


        [Route("Register")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<object> Register(UserRegisterModel userRegisterModel)
        {
            var result = await _userService.Register(userRegisterModel);
            if (result is IdentityResult identityResult)
            {
                if (identityResult.Succeeded)
                {
                    return Ok("User registered successfully.");
                }
                else
                {
                    var errors = identityResult.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }
            }

            if (result?.GetType().GetProperty("Error")?.GetValue(result) is string errorDetail)
            {
                return BadRequest(new { Error = errorDetail });
            }

            if (result?.GetType().GetProperty("Message")?.GetValue(result) is string message)
            {
                return BadRequest(new { Message = message });
            }

            return BadRequest("Unknown error occurred.");
        }

            [AllowAnonymous]
            [HttpPost("authenticate")]
            public async Task<IActionResult> Authenticate(UserAuthenticateModel model)
            {
                var response = await _userService.AuthenticateUser(model);

                if (response is BadRequestObjectResult badRequestResult)
                {
                    return BadRequest(badRequestResult.Value);
                }

                if (response is OkObjectResult okResponse)
                {
                    return Ok(okResponse.Value);
                }

                return BadRequest("An unknown error occurred.");
            }

            [HttpGet("UserCheck")]
            [Authorize]
            public ActionResult<string> GetMyName()
            {
                var user = User.FindFirstValue(ClaimTypes.Email);
                return user;
            }

            [HttpGet]
            [Authorize]
            public IActionResult Get()
            {
                var emailClaim = User.Claims.First(c => c.Type == ClaimTypes.Email).Value;

                if (emailClaim != null)
                {
                    return Ok(new { Email = emailClaim });
                }
                else
                {
                    return NotFound("Email not found in claims.");
                }
            }

            [HttpGet]
            public IActionResult ExternalLogin(string returnUrl = "/")
            {
            var props = _userService.GetAuthenticationProperties(returnUrl);
            
            return Challenge(props, GoogleDefaults.AuthenticationScheme);
            }

            [HttpGet]
            public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
            {
                var user = await _userService.HandleCallbackAsync();

                var jwt = _jwtService.GenerateToken(user);

                return Ok(new { token = jwt, returnUrl });
            }
    }
} 
