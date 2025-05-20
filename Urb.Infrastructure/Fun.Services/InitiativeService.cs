using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly ICRUDRepository<Category> _categoryRepo;

        public InitiativeService(
            IUserService userService,
            ICRUDRepository<Initiative> repo,
            IHttpContextAccessor httpCtx,
            IWebHostEnvironment env,
            IMapper mapper,
            ICRUDRepository<Category> cRUD)
        {
            _userService = userService;
            _env = env;
            _repo = repo;
            _httpCtx = httpCtx;
            _mapper = mapper;
            _categoryRepo = cRUD;
        }

        private string CurrentUserId =>
            _httpCtx.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value!;

        public async Task<IInitiativeComponentModel> CreateAsync(IInitiativeComponentModel initiativeComponent)
        {
            var entity = _mapper.Map<Initiative>(initiativeComponent);
            entity.UserId =  await _userService.GetMy();
            entity.ImageUrl = await SaveImageAsync(initiativeComponent.ImageFile);
            var created = await _repo.Create(entity);
            var result = _mapper.Map<IInitiativeComponentModel>(created);
            return result;
        }

        public Task<Initiative?> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public async Task<IEnumerable<Initiative>> ListAsync()
        {
            var all = await (_repo as dynamic).ListAsync();
            return all;
        }

        //public async Task<IInitiativeComponentModel> UpdateAsync(IInitiativeComponentModel initiativeComponent)
        //{
        //    var existing = await _repo.GetByIdAsync(initiativeComponent.Id);
        //    if (existing == null) throw new KeyNotFoundException();
        //    if (existing.UserId != CurrentUserId)
        //        throw new UnauthorizedAccessException();

        //    dto.UserId = existing.UserId;
        //    dto.CreatedDate = existing.CreatedDate;
        //    return await _repo.Put(dto);
        //}

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) throw new KeyNotFoundException();
            //if (existing.UserId != CurrentUserId)
            //    throw new UnauthorizedAccessException();
            await _repo.Delete(id);
        }
        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var fileType = Path.GetExtension(file.FileName);
            var uniqueFilename = $"{fileName}_{Guid.NewGuid()}{fileType}";

            var structPath = Path.Combine("images", "initiatives", uniqueFilename);
            var finalPath = Path.Combine(_env.WebRootPath, structPath);

            using var stream = (new FileStream(finalPath, FileMode.Create));
            {
                await file.CopyToAsync(stream);
            }
            return "/" + finalPath.Replace("\\", "/");
        }

        //public async Task<IEnumerable<Initiative>> GetByCategoryNamesAsync(IEnumerable<string> categoryNames)
        //{
        //    var all = await _repo.ListAsync();
        //    if (categoryNames == null || !categoryNames.Any())
        //        return all;
        //    return all.Where(i =>
        //        i.Category != null &&
        //        categoryNames.Contains(i.Category.CategoryName));
        //}
        public async Task<IEnumerable<Initiative>> GetByCategoryNamesAsync(IEnumerable<string> categoryNames)
        {
            if (categoryNames == null || !categoryNames.Any())
                return await _repo.ListAsync();

            // 1) Знаходимо всі категорії з такими назвами
            var allCats = await _categoryRepo.ListAsync();
            var ids = allCats
                .Where(c => categoryNames.Contains(c.CategoryName))
                .Select(c => c.Id)
                .ToHashSet();

            // 2) Фільтруємо ініціативи по CategoryId
            var allInits = await _repo.ListAsync();
            return allInits
                .Where(i => ids.Contains(i.CategoryId));
        }

        public async Task<IEnumerable<Initiative>> GetByUserAsync(int userId)
        {
            var all = await _repo.ListAsync();
            return all.Where(i => i.UserId == userId);
        }
        //public async Task<InitiativeStatisticsComponentModel> GetStatisticsAsync(int initiativeId)
        //{
        //    var initiative = await _db.Initiatives
        //        .AsNoTracking()
        //        .Include(i => i.Stat)
        //        .Include(i => i.Subscribes)
        //        .Include(i => i.Fundraisings)
        //            .ThenInclude(f => f.Stat)
        //        .ThenInclude(s => s.DailyIncomes)
        //        .FirstOrDefaultAsync(i => i.Id == initiativeId);

        //    if (initiative is null)
        //        throw new KeyNotFoundException($"Initiative #{initiativeId} not found.");

        //    return new InitiativeStatisticsComponentModel
        //    {
        //        InitiativeId = initiative.Id,
        //        Title = initiative.Title,
        //        TotalViews = initiative.Stat.TotalViews,
        //        TotalFundraisings = initiative.Stat.TotalFundraisings,
        //        TotalSubscribers = initiative.Stat.TotalSubscribers,
        //        Fundraisings = initiative.Fundraisings.Select(f => new FundraisingStatisticsComponentModel
        //        {
        //            FundraisingId = f.Id,
        //            Name = f.Name,
        //            Goal = f.Stat.Goal,
        //            TotalCollected = f.Stat.TotalCollected,
        //            DailyIncomes = f.Stat.DailyIncomes
        //                .OrderBy(di => di.Date)
        //                .Select(di => new DailyIncomeDto
        //                {
        //                    Date = di.Date,
        //                    Amount = di.Amount
        //                })
        //                .ToList()
        //        }).ToList()
        //    };
        //}
    }
}
