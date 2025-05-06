using Fun.Application.Fun.IServices;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class FundraisingsController : ControllerBase
        {
            private readonly IFundraisingService _svc;
            public FundraisingsController(IFundraisingService svc) => _svc = svc;

            [HttpGet("byInitiative/{initiativeId}")]
            public async Task<IActionResult> GetByInitiative(string initiativeId)
                => Ok(await _svc.ListAsync(initiativeId));

            [HttpGet("{id}")]
            public async Task<IActionResult> Get(string id)
            {
                var f = await _svc.GetByIdAsync(id);
                return f == null ? NotFound() : Ok(f);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] Fundraising dto)
            {
                try
                {
                    var created = await _svc.CreateAsync(dto);
                    return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }

            [HttpPut("{id}")]
            public async Task<IActionResult> Update(string id, [FromBody] Fundraising dto)
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
