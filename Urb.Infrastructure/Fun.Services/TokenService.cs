using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Urb.Application.App.Settings;
using Urb.Domain.Urb.Models;

namespace Fun.Infrastructure.Fun.Services
{
    public class TokenService: ITokenService
    {
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;
        //private readonly IUserAuthenticateModel _userAuthenticateModel;

        public TokenService(IOptions<AppSettings> appSettings, /*User user,*/ IConfiguration configuration/*, IUserAuthenticateModel userAuthenticateModel*/)
        {
            _configuration = configuration;
            _appSettings = appSettings.Value;
            //_userAuthenticateModel = userAuthenticateModel;
        }

        public string GenerateToken(IUserAuthenticateModel model/*IdentityUser user*//*User model*/)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_appSettings.JWTKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {                     
                new Claim(ClaimTypes.Email, model.Email.ToString()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string? ValidateToken(string token)
        {
            if (token == null)
            {
                return null;
            }
            AppSettings appSettings = new AppSettings();
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(appSettings.JWTKey);
            try
            {
                jwtSecurityTokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userEmail = jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;

                return userEmail;
            }
            catch
            {
                return null;
            }
        }
    }
}
