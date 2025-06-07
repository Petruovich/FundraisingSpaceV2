using AutoMapper;
using Fun.Application.ComponentModels;
using Fun.Application.Fun.IRepositories;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Application.ResponseModels;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Urb.Infrastructure.Fun.Services;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitiativeController : ControllerBase
    {
        private readonly IInitiativeService _svc;
        private readonly ICRUDRepository<Initiative> _initiativeRepo;
        private readonly ICRUDRepository<Category> _categoryRepo;
        private readonly IMapper _mapper;
        //public InitiativesController(IInitiativeService svc) => _svc = svc;
        public InitiativeController(IInitiativeService svc, ICRUDRepository<Initiative> initiativeRepo, ICRUDRepository<Category> categoryRepo, IMapper mapper)
        {
            _svc = svc ?? throw new ArgumentNullException(nameof(svc));
            _initiativeRepo = initiativeRepo ?? throw new ArgumentNullException(nameof(initiativeRepo));
            _categoryRepo = categoryRepo;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.ListAsync());

        //[HttpGet]
        //[Route("GetInitiativeById")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    var init = await _svc.GetByIdAsync(id);
        //    return init == null ? NotFound() : Ok(init);
        //}
        [HttpGet("GetInitiativeById")]
        [AllowAnonymous]
        public async Task<ActionResult<InitiativeDetailResponseModel>> GetById(int id)
        {
            try
            {
                var dto = await _svc.GetByIdWithFundraisingsAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Initiative #{id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Server error: " + ex.Message });
            }
        }


        [Authorize]
        [Route("AddInitiative")]
        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task</*IActionResult*/object> Create([FromForm] InitiativeComponentModel initiativeComponentModel)
        {
            var created = await _svc.CreateAsync(initiativeComponentModel);
            return created;/*CreatedAtAction(nameof(Get), new { id = created.Id }, created);*/
        }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] Initiative dto)
        //{
        //    if (id != dto.Id) return BadRequest();
        //    try
        //    {
        //        var updated = await _svc.UpdateAsync(dto);
        //        return Ok(updated);
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        return Forbid();
        //    }
        //}

        [HttpDelete("InitiativeDelete")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _svc.DeleteAsync(id);
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpGet("byCategories")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategories([FromQuery] List<string> categoryNames)
        {
            var entities = await _svc.GetByCategoryNamesAsync(categoryNames);

            var dtos = new List<InitiativeResponseModel>(entities.Count());
            foreach (var e in entities)
            {
                var dto = await _svc.ToResponseModelAsync(e);
                dtos.Add(dto);
            }

            return Ok(dtos);
        }

        [HttpGet("byCategoriesWithPagin")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCategories(
    [FromQuery] List<string> categoryNames,
    [FromQuery] int page = 1)
        {
            const int PageSize = 15;
            var all = await _svc.GetByCategoryNamesAsync(categoryNames);
                        var paged = all
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();
                        var dtos = new List<InitiativeResponseModel>(paged.Count);
            foreach (var e in paged)
            {
                var dto = await _svc.ToResponseModelAsync(e);
                dtos.Add(dto);
            }
            return Ok(new
            {
                page,
                pageSize = PageSize,
                totalCount = all.Count(),  
                items = dtos
            });
        }

        [HttpPut("EditInitiative")]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [FromForm] InitiativeEditComponentModel model)
        {
            try
            {
                await _svc.EditInitiativeAsync(id, model);
                return Ok(new { Message = $"Initiative #{id} has been updated." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Initiative #{id} not found." });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Unexpected error. " + ex.Message });
            }
        }

        //[HttpGet("{id}/statistics")]
        //public async Task<ActionResult<InitiativeStatisticsComponentModel>> GetStatistics(int id)
        //{
        //    try
        //    {
        //        var stats = await _srv.GetStatisticsAsync(id);
        //        return Ok(stats);
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return NotFound();
        //    }
        //}
        [HttpGet("InitiativeStat")]
        [AllowAnonymous]
        public async Task<ActionResult<List<InitiativeStatResponseModel>>> GetDailyStats(int id)
        {
            try
            {
                var stats = await _svc.GetDailyStatsAsync(id);
                return Ok(stats);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Initiative #{id} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Server error: " + ex.Message });
            }
        }
        [HttpGet("InitiativeMonth-YearStat")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOverview(int id)
        {
            try
            {
                var dto = await _svc.GetOverviewAsync(id);
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = $"Initiative #{id} not found." });
            }
        }
    }
}

