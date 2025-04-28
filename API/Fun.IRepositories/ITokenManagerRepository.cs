using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urb.Application.Urb.IRepositories
{
    public interface ITokenManagerRepository
    {
        Task<bool> IsTokenValid(string token);
        Task<bool> IsTokenExpired(string token);
        Task<bool> IsTokenRevoked(string token);
        Task<bool> RevokeToken(string token);
        Task<bool> AddToken(string token);
        Task<bool> RemoveToken(string token);
        Task<string> GetUserIdFromToken(string token);
        Task<string> GetUserNameFromToken(string token);
        Task<string> GetUserEmailFromToken(string token);
    }
}
