using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Infrastructure.Fun.Services
{
    public class FundraisingService /*: IFundraisingService*/
    {
        private readonly ICRUDRepository<Fundraising> _repo;
        private readonly MainDataContext _ctx;
        private readonly IHttpContextAccessor _httpCtx;

        public FundraisingService(
            ICRUDRepository<Fundraising> repo,
            MainDataContext ctx,
            IHttpContextAccessor httpCtx)
        {
            _repo = repo;
            _ctx = ctx;
            _httpCtx = httpCtx;
        }

        private string CurrentUserId =>
            _httpCtx.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!;

        public async Task<Fundraising> CreateAsync(Fundraising dto)
        {
            var init = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == dto.InitiativeId);
            if (init == null) throw new KeyNotFoundException("Initiative not found");
            if (init.UserId != CurrentUserId)
                throw new UnauthorizedAccessException();

            return await _repo.Create(dto);
        }

        public Task<Fundraising?> GetByIdAsync(string id)
            => _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Fundraising>> ListAsync(string initiativeId)
        {
            return await _ctx.Fundraisings
                .AsNoTracking()
                .Where(f => f.InitiativeId == initiativeId)
                .ToListAsync();
        }

        public async Task<Fundraising> UpdateAsync(Fundraising dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) throw new KeyNotFoundException();

            var init = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == existing.InitiativeId);
            if (init == null) throw new KeyNotFoundException("Initiative not found");
            if (init.UserId != CurrentUserId)
                throw new UnauthorizedAccessException();

            return await _repo.Put(dto);
        }

        public async Task DeleteAsync(string id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException();
            var init = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == existing.InitiativeId);
            if (init == null) throw new KeyNotFoundException("Initiative not found");
            if (init.UserId != CurrentUserId)
                throw new UnauthorizedAccessException();

            await _repo.Delete(id);
        }

        public async Task<IEnumerable<Fundraising>> GetByUserAsync(int userId)
        {
            var userInitiatives = await _initiatives.GetByUserAsync(userId);
            var ids = userInitiatives.Select(x => x.Id).ToHashSet();
            var all = await _repo.ListAsync();
            return all.Where(f => ids.Contains(f.InitiativeId));
        }
    }
}
