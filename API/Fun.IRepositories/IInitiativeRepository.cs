using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IRepositories
{
    public interface IInitiativeRepository
    {
        Task<bool> IsInitiativeValid(string initiativeId);
        Task<bool> IsInitiativeExpired(string initiativeId);
        Task<bool> IsInitiativeRevoked(string initiativeId);
        Task<bool> RevokeInitiative(string initiativeId);
        Task<bool> AddInitiative(string initiativeId);
        Task<bool> RemoveInitiative(string initiativeId);
        Task<string> GetUserIdFromInitiative(string initiativeId);
        Task<string> GetUserNameFromInitiative(string initiativeId);
        Task<string> GetUserEmailFromInitiative(string initiativeId);
    }
}
