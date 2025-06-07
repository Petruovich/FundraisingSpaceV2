using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Application.ResponseModels;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Stripe;
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
        private readonly MainDataContext _db;

        public InitiativeService(
            IUserService userService,
            ICRUDRepository<Initiative> repo,
            IHttpContextAccessor httpCtx,
            IWebHostEnvironment env,
            IMapper mapper,
            ICRUDRepository<Category> cRUD,
            MainDataContext db)
        {
            _userService = userService;
            _env = env;
            _repo = repo;
            _httpCtx = httpCtx;
            _mapper = mapper;
            _categoryRepo = cRUD;
            _db = db;
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

            var allCats = await _categoryRepo.ListAsync();
            var ids = allCats
                .Where(c => categoryNames.Contains(c.CategoryName))
                .Select(c => c.Id)
                .ToHashSet();

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

        public async Task<InitiativeResponseModel> ToResponseModelAsync(Initiative entity)
        {
            var dto = _mapper.Map<InitiativeResponseModel>(entity);

            if (!string.IsNullOrEmpty(entity.ImageUrl))
            {
                var relative = entity.ImageUrl.TrimStart('/');
                var wwwRoot = _env.WebRootPath;
                var fullPath = Path.Combine(wwwRoot, relative);
                if (!System.IO.File.Exists(fullPath))
                    throw new FileNotFoundException();

                using var stream = System.IO.File.OpenRead(fullPath);

                var memory = new MemoryStream();
                await stream.CopyToAsync(memory);
                memory.Position = 0;
                dto.ImageBase64 = Convert.ToBase64String(memory.ToArray());

                //var fileName = Path.GetFileName(relative);
                //var ext = Path.GetExtension(relative).TrimStart('.');
                //var contentType = ext.Equals("png", StringComparison.OrdinalIgnoreCase) ? "image/png"
                //                : ext.Equals("jpg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg"
                //                : ext.Equals("jpeg", StringComparison.OrdinalIgnoreCase) ? "image/jpeg"
                //                : "application/octet-stream";

                //dto.ImageFile = new FormFile(memory, 0, memory.Length, "ImageFile", fileName)
                //{
                //    Headers = new HeaderDictionary(),
                //    ContentType = contentType
                //};
            }
            return dto;
        }
        //public Task<InitiativeResponseModel> ToResponseModelAsync(Initiative entity)
        //{
        //    var dto = _mapper.Map<InitiativeResponseModel>(entity);
        //    return Task.FromResult(dto);
        //}


        public async Task EditInitiativeAsync(int id, InitiativeEditComponentModel model)
        {
           
            var initiative = await _db.Initiatives
                .FirstOrDefaultAsync(i => i.Id == id);

            if (initiative == null)
                throw new KeyNotFoundException($"Initiative #{id} not found.");

           
            var currentUserId = await _userService.GetMy(); 
            if (initiative.UserId != currentUserId /* && !UserIsAdmin(...) */)
                throw new UnauthorizedAccessException("You are not allowed to edit this initiative.");


            if (!string.IsNullOrWhiteSpace(model.Title))
            {
                initiative.Title = model.Title!.Trim();
            }


            if (!string.IsNullOrWhiteSpace(model.Description))
            {
                initiative.Description = model.Description!.Trim();
            }


            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {

                if (!string.IsNullOrEmpty(initiative.ImageUrl))
                {
                    var oldRelative = initiative.ImageUrl.TrimStart('/');
                    var oldAbsolute = Path.Combine(_env.WebRootPath, oldRelative);
                    if (System.IO.File.Exists(oldAbsolute))
                    {
                        try
                        {
                            System.IO.File.Delete(oldAbsolute);
                        }
                        catch
                        {
                           
                        }
                    }
                }

                var newImageUrl = await SaveImageAsync(model.ImageFile);
                initiative.ImageUrl = newImageUrl;
            }
            await _db.SaveChangesAsync();
        }

        public async Task<InitiativeDetailResponseModel> GetByIdWithFundraisingsAsync(int id)
        {
            var initiative = await _db.Initiatives
                .Include(i => i.Fundraisings)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (initiative == null)
                throw new KeyNotFoundException($"Initiative #{id} not found.");

            string? imageBase64 = null;
            if (!string.IsNullOrEmpty(initiative.ImageUrl))
            {
                var relative = initiative.ImageUrl.TrimStart('/');
                var fullPath = Path.Combine(_env.WebRootPath, relative);
                if (System.IO.File.Exists(fullPath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(fullPath);

                    var ext = Path.GetExtension(fullPath).TrimStart('.').ToLowerInvariant();
                    var mime = ext switch
                    {
                        "png" => "image/png",
                        "jpg" => "image/jpeg",
                        "jpeg" => "image/jpeg",
                        "gif" => "image/gif",
                        _ => "application/octet-stream"
                    };
                    var b64 = Convert.ToBase64String(bytes);
                    imageBase64 = $"data:{mime};base64,{b64}";
                }
            }

            var frList = initiative.Fundraisings
                .Select(f => new FundraisingResponseModel
                {
                    Id = f.Id,
                    Title = f.Title,
                    GoalAmount = f.GoalAmount,
                    CreatedAt = f.CreatedAt,
                    Deadline = f.Deadline
                })
                .ToList();

            return new InitiativeDetailResponseModel
            {
                Id = initiative.Id,
                Title = initiative.Title,
                Description = initiative.Description,
                CategoryId = initiative.CategoryId,
                ImageBase64 = imageBase64,
                Fundraisings = frList
            };
        }
    }
}
