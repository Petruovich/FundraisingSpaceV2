using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Application.ResponseModels;
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

        public async Task<List<DonorResponseModel>> GetTopDonorsAsync(int fundraisingId)
        {
            var grouped = await _db.Donates
                .Where(d => d.FundraisingId == fundraisingId)
                .GroupBy(d => d.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();
            if (grouped.Count == 0)
                return new List<DonorResponseModel>();

            var userIds = grouped.Select(x => x.UserId).ToList();
            var users = await _db.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.AvatarUrl
                })
                .ToListAsync();

            var result = new List<DonorResponseModel>(grouped.Count);
            foreach (var item in grouped)
            {
                var usr = users.FirstOrDefault(u => u.Id == item.UserId);
                if (usr == null)
                    continue; 

                string normalizedName;
                if (!string.IsNullOrWhiteSpace(usr.UserName) && usr.UserName.Contains("."))
                {
                    var parts = usr.UserName.Split('.', StringSplitOptions.RemoveEmptyEntries);
                    normalizedName = string.Join(" ", parts);
                }
                else
                {
                    normalizedName = usr.UserName ?? string.Empty;
                }

                string? avatarBase64 = null;
                if (!string.IsNullOrEmpty(usr.AvatarUrl))
                {
                    var relativePath = usr.AvatarUrl.TrimStart('/');
                    var wwwroot = Path.Combine(AppDomain.CurrentDomain.GetData("ContentRootPath")?.ToString() ?? string.Empty, "wwwroot");
                    var fullPath = Path.Combine(wwwroot, relativePath);

                    if (File.Exists(fullPath))
                    {
                        var bytes = await File.ReadAllBytesAsync(fullPath);
                        var ext = Path.GetExtension(fullPath).ToLowerInvariant().TrimStart('.');
                        var mime = ext switch
                        {
                            "png" => "image/png",
                            "jpg" => "image/jpeg",
                            "jpeg" => "image/jpeg",
                            "gif" => "image/gif",
                            _ => "application/octet-stream"
                        };
                        var base64Data = Convert.ToBase64String(bytes);
                        avatarBase64 = $"data:{mime};base64,{base64Data}";
                    }
                }
                result.Add(new DonorResponseModel
                {
                    UserId = item.UserId,
                    NormalizedName = normalizedName,
                    AvatarBase64 = avatarBase64,
                    TotalAmount = item.TotalAmount
                });
            }
            return result;
        }
    }
}
