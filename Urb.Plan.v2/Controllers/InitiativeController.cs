using Fun.Application.Fun.IServices;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
            => Ok(await _svc.ListAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var init = await _svc.GetByIdAsync(id);
            return init == null ? NotFound() : Ok(init);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Initiative dto)
        {
            var created = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
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
    }
}

