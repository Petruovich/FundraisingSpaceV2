using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Urb.Infrastructure.Fun.Services
{
    public class InitiativeService : IInitiativeService
    {
        private readonly ICRUDRepository<Initiative> _repo;
        private readonly IHttpContextAccessor _httpCtx;

        public InitiativeService(
            ICRUDRepository<Initiative> repo,
            IHttpContextAccessor httpCtx)
        {
            _repo = repo;
            _httpCtx = httpCtx;
        }

        private string CurrentUserId =>
            _httpCtx.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!;

        public async Task<Initiative> CreateAsync(Initiative dto)
        {
            dto.UserId = CurrentUserId;
            dto.CreatedDate = DateTime.UtcNow;
            return await _repo.Create(dto);
        }

        public Task<Initiative?> GetByIdAsync(string id)
            => _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Initiative>> ListAsync()
        {
            var all = await (_repo as dynamic).ListAsync();
            return all;
        }

        public async Task<Initiative> UpdateAsync(Initiative dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) throw new KeyNotFoundException();
            if (existing.UserId != CurrentUserId)
                throw new UnauthorizedAccessException();

            dto.UserId = existing.UserId;
            dto.CreatedDate = existing.CreatedDate;
            return await _repo.Put(dto);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException();
            if (existing.UserId != CurrentUserId)
                throw new UnauthorizedAccessException();

            await _repo.Delete(id);
        }
    }
}
