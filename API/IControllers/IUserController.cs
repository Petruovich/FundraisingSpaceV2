using Fun.Application.ComponentModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.IControllers
{
    public interface IUserController
    {
        public Task<object> Register(UserRegisterModel userRegisterModel);
        public Task<IActionResult> Authenticate(UserAuthenticateModel model);
        public ActionResult<string> GetMyName();
        public IActionResult Get();
    }
}
