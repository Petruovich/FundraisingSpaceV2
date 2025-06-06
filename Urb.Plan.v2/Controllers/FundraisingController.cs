using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Domain.Fun.Models;
using Fun.Infrastructure.Fun.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
        public class FundraisingsController : ControllerBase
        {
            private readonly IFundraisingService _svc;
            private readonly IHttpContextAccessor _httpCtx;
        public FundraisingsController(IFundraisingService svc) /*=> _svc = svc;*/
        {
            _svc = svc;
        }

            //[HttpGet("byInitiative/{initiativeId}")]
            //public async Task<IActionResult> GetByInitiative(string initiativeId)
            //    => Ok(await _svc.ListAsync(initiativeId));

            [HttpGet("{id}")]
            public async Task<IActionResult> Get(int id)
            {
                var f = await _svc.GetByIdAsync(id);
                return f == null ? NotFound() : Ok(f);
            }

            [HttpPost]
            [Authorize]
            public async Task<IActionResult> Create([FromBody] FundraisingComponentModel fundraisingComponentModel)
            {
                try
                {
                    var created = await _svc.CreateAsync(fundraisingComponentModel);
                    return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
            }

        //[HttpPut("{id}")]
        //public async Task<IActionResult> Update(string id, [FromBody] Fundraising dto)
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
        
            [HttpDelete("FundraisingDelete")]
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

        [AllowAnonymous]
        [HttpGet("byInitiative/{initiativeId:int}")]
        public async Task<ActionResult<IEnumerable<Fundraising>>> GetByInitiative(int initiativeId)
            {
                var list = await _svc.GetByInitiativeAsync(initiativeId);
                return Ok(list);
            }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<FundraisingStatisticsComponentModel>> GetStatistics(int id)
        {
            try
            {
                var stats = await _svc.GetStatisticsAsync(id);
                return Ok(stats);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
        }        
}
