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
        Task<Fundraising?> GetByIdAsync(string id);
        Task<IEnumerable<Fundraising>> ListAsync(string initiativeId);
        Task<Fundraising> UpdateAsync(Fundraising dto);
        Task DeleteAsync(string id);
    }
}
