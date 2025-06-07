using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IServices
{
    public interface IVerificationService
    {
        Task<bool> ConfirmEmailAsync(string token);
        Task<bool> VerifyTwoFactorAsync(string code);
    }
}
