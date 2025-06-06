using Fun.Application.Fun.IRepositories;
using Fun.Domain.Fun.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Persistance.Fun.Repositories
{
    public class CRUDRepository<T>: ICRUDRepository<T>
        where T : class
    {        
        private readonly MainDataContext _context;
        private readonly DbSet<T> _set;
        public CRUDRepository(MainDataContext context)
        {
            _context = context;
            _set = _set = context.Set<T>();
        }
        public async Task<T> Create(T entity)
        {
            _set.Add(entity);
            await _context.SaveChangesAsync();
            return entity;         
        }
        public async Task Delete(int id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity not found");
            }
            _context.Remove(entity);
            await _context.SaveChangesAsync();           
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _set.FindAsync(id);
        }
        public async Task<IEnumerable<T>> ListAsync()
        {
            return await _set.ToListAsync();
        }
        public async Task<T> Put(T entity)
        {
            _set.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }               
    }
}