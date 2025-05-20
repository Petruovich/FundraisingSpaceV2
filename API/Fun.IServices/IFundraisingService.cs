using Fun.Application.ComponentModels;
using Fun.Domain.Fun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IServices
{
    public interface IFundraisingService
    {
        Task<Fundraising> CreateAsync(Fundraising dto);
        Task<Fundraising?> GetByIdAsync(int id);
        //Task<IEnumerable<Fundraising>> ListAsync(string initiativeId);
        Task<Fundraising> UpdateAsync(Fundraising dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<Fundraising>> GetByInitiativeAsync(int initiativeId);
        Task<FundraisingStatisticsComponentModel> GetStatisticsAsync(int fundraisingId);
    }
}
