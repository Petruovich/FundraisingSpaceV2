using Fun.Application.IComponentModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Urb.Domain.Urb.Models;

namespace Fun.Application.Fun.IServices
{
    public interface ITokenService
    {
        public string GenerateToken(IUserAuthenticateModel userauth/*User model*/);
        public string? ValidateToken(string token);
        //public string GenerateToken_2(IUserAuthenticateModel model);
    }
}
