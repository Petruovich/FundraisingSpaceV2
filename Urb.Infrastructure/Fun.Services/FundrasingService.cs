using AutoMapper;
using Fun.Application.ComponentModels;
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
using Urb.Infrastructure.Fun.Services;

namespace Fun.Infrastructure.Fun.Services
{
    public class FundraisingService: IFundraisingService
    {
        private readonly ICRUDRepository<Fundraising> _repo;
        private readonly MainDataContext _ctx;
        private readonly IHttpContextAccessor _httpCtx;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public FundraisingService(
            ICRUDRepository<Fundraising> repo,
            MainDataContext ctx,
            IHttpContextAccessor httpCtx,
            IUserService userService, IMapper mapper)
        {
            _repo = repo;
            _ctx = ctx;
            _httpCtx = httpCtx;
            _userService = userService;
            _mapper = mapper;
        }

        private string CurrentUserId =>
            _httpCtx.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!;

        public async Task<Fundraising> CreateAsync(FundraisingComponentModel dto)
        {
            var initiative = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == dto.InitiativeId);
            if (initiative == null)
                throw new KeyNotFoundException($"Initiative #{dto.InitiativeId} not found.");

            var entity = _mapper.Map<Fundraising>(dto);

            entity.CreatedAt = DateTime.UtcNow;
            entity.CreatedById = await _userService.GetMy();
            entity.Initiative = initiative;

            var created = await _repo.Create(entity);

            var stat = new FundraisingStat
            {
                FundraisingId = created.Id,
                TotalCollected = 0m,
                //CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _ctx.FundraisingStats.Add(stat);
            await _ctx.SaveChangesAsync();

            return created;
        }

        public Task<Fundraising?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);      

        public async Task<Fundraising> UpdateAsync(Fundraising dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null) throw new KeyNotFoundException();

            var init = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == existing.InitiativeId);
            if (init == null) throw new KeyNotFoundException("Initiative not found");

            return await _repo.Put(dto);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException();
            var init = await _ctx.Initiatives
                .FirstOrDefaultAsync(i => i.Id == existing.InitiativeId);
            if (init == null) throw new KeyNotFoundException("Initiative not found");
            await _repo.Delete(id);
        }

        //public async Task<IEnumerable<Fundraising>> GetByUserAsync(int userId)
        //{
        //    var userInitiatives = await _ctx.Initiatives.G GetByUserAsync(userId);
        //    var ids = userInitiatives.Select(x => x.Id).ToHashSet();
        //    var all = await _repo.ListAsync();
        //    return all.Where(f => ids.Contains(f.InitiativeId));
        //}

        public async Task<IEnumerable<Fundraising>> GetByInitiativeAsync(int initiativeId)
        {
            var all = await _repo.ListAsync();
            return all.Where(f => f.InitiativeId == initiativeId);
        }

        public async Task<FundraisingStatisticsComponentModel> GetStatisticsAsync(int fundraisingId)
        {
            var fund = await _ctx.Fundraisings
                .AsNoTracking()
                .Include(f => f.Stat)
                    .ThenInclude(s => s.DailyIncomes)
                .FirstOrDefaultAsync(f => f.Id == fundraisingId);

            if (fund is null)
                throw new KeyNotFoundException($"Fundraising #{fundraisingId} not found.");

            var stat = fund.Stat;
            return new FundraisingStatisticsComponentModel
            {
                FundraisingId = fund.Id,
                Goal = /*stat.Goal*/fund.GoalAmount, 
                TotalCollected = stat.TotalCollected,
                UpdatedAt = stat.UpdatedAt,
                DailyIncomes = stat.DailyIncomes
                    .OrderBy(d => d.Date)
                    .Select(d => new FundraisingDailyIncomeComponentModel
                    {
                        Date = d.Date,
                        Amount = d.Amount
                    })
                    .ToList()
            };
        }
    }
}
