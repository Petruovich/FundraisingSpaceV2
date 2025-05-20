using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Infrastructure.Fun.Services
{
    public class DonateService: IDonateService
    {
        private readonly ICRUDRepository<Fundraising> _repo;
        private readonly MainDataContext _db;
        private readonly IHttpContextAccessor _httpCtx;

        public DonateService(
            ICRUDRepository<Fundraising> repo,
            MainDataContext db,
            IHttpContextAccessor httpCtx)
        {
            _repo = repo;
            _httpCtx = httpCtx;
            _db = db;
        }
        public async Task DonateAsync(int fundraisingId, decimal amount, int userId)
        {
            var donation = new Donate
            {
                FundraisingId = fundraisingId,
                UserId = userId,
                Amount = amount,
                Date = DateTime.UtcNow
            };
            _db.Donates.Add(donation);

            var stat = await _db.FundraisingStats
                .FirstOrDefaultAsync(s => s.FundraisingId == fundraisingId)
                ?? throw new KeyNotFoundException($"FundraisingStat for #{fundraisingId} not found.");
            stat.TotalCollected += amount;
            stat.UpdatedAt = DateTime.UtcNow;


            var today = DateTime.UtcNow.Date;
            var daily = await _db.FundraisingDailyIncomes
                .FirstOrDefaultAsync(d => d.FundraisingStatId == stat.Id && d.Date == today);
            if (daily == null)
            {
                _db.FundraisingDailyIncomes.Add(new FundraisingDailyIncome
                {
                    FundraisingStatId = stat.Id,
                    Date = today,
                    Amount = amount
                });
            }
            else
            {
                daily.Amount += amount;
            }

            await _db.SaveChangesAsync();
        }
    }
}
