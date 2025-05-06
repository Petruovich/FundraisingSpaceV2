using Fun.Domain.Fun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IServices
{
    public interface IInitiativeService
    {
        Task<Initiative> CreateAsync(Initiative dto);
        Task<Initiative?> GetByIdAsync(string id);
        Task<IEnumerable<Initiative>> ListAsync();
        Task<Initiative> UpdateAsync(Initiative dto);
        Task DeleteAsync(string id);
    }
}
