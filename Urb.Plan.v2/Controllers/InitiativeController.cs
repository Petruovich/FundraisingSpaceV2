using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InitiativeController : ControllerBase
    {
        private readonly IInitiativeService _svc;
        //public InitiativesController(IInitiativeService svc) => _svc = svc;
        public InitiativeController(IInitiativeService svc)
        {
            _svc = svc ?? throw new ArgumentNullException(nameof(svc));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.ListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var init = await _svc.GetByIdAsync(id);
            return init == null ? NotFound() : Ok(init);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Initiative dto)
        {
            if (id != dto.Id) return BadRequest();
            try
            {
                var updated = await _svc.UpdateAsync(dto);
                return Ok(updated);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
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

        [HttpGet("byCategory/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<Initiative>>> GetByCategory(int categoryId)
        {
            var items = await _svc.GetByCategoryAsync(categoryId);
            return Ok(items);
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<InitiativeStatisticsDto>> GetStatistics(int id)
        {
            try
            {
                var stats = await _srv.GetStatisticsAsync(id);
                return Ok(stats);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

