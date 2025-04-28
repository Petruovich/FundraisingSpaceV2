using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IRepositories
{
    public interface IFundraisingRepository
    {
        //Task<bool> IsFundraisingValid(string fundraisingId);
        //Task<bool> IsFundraisingExpired(string fundraisingId);
        //Task<bool> IsFundraisingRevoked(string fundraisingId);
        Task<bool> RevokeFundraising(string fundraisingId);
        Task<bool> AddFundraising(string fundraisingId);       
        Task<string> GetUserIdFromFundraising(string fundraisingId);
        Task<string> GetUserNameFromFundraising(string fundraisingId);
        Task<string> GetUserEmailFromFundraising(string fundraisingId);
    }
}
