using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.Fun.IRepositories
{
    public interface ICRUDRepository<T> 
    {
        Task<T> Create(T entity);
        Task<T> Put(T entity);        
        Task Delete(string id);
        Task<T?> GetByIdAsync(string id);
    }
}
