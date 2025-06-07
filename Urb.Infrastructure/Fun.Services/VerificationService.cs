using Fun.Application.Fun.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Infrastructure.Fun.Services
{
    public class VerificationService: IVerificationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VerificationService(
            UserManager<User> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        private Task<User> GetCurrentUserAsync() =>
            _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

        public async Task<bool> ConfirmEmailAsync(string encodedToken)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return false;

            var tokenBytes = WebEncoders.Base64UrlDecode(encodedToken);
            var token = Encoding.UTF8.GetString(tokenBytes);

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result.Succeeded;
        }

        public async Task<bool> VerifyTwoFactorAsync(string code)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return false;

            var result = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                TokenOptions.DefaultAuthenticatorProvider,
                code);
            return result;
        }
    }
}
