using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Urb.Plan.v2.IServices;
using Urb.Plan.v2.Views;

namespace Urb.Plan.v2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController: ControllerBase
    {
        private IUserService _userService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;       
        public UserController(
            IUserService userService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }


        [Route("Register")]
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(UserRegisterModel userRegisterModel)
        {
            _userService.Register(userRegisterModel);
            return Ok(new { message = "Registration successful" });
        }

        [Route("GetAll")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [Route("GetUser")]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetUser(id);
            return Ok(user);
        }
    }
}
