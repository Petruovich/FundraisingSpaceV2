using Fun.Application.ComponentModels;
using Fun.Application.IComponentModels;
using Fun.Application.ResponseModels;
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
        Task<IInitiativeComponentModel> CreateAsync(IInitiativeComponentModel dto);
        Task<Initiative?> GetByIdAsync(int id);
        Task<IEnumerable<Initiative>> ListAsync();
        Task DeleteAsync(int id);
        Task<IEnumerable<Initiative>> GetByCategoryNamesAsync(IEnumerable<string> categoryNames);
        Task<InitiativeResponseModel> ToResponseModelAsync(Initiative entity);
    }
}
