using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Application.IComponentModels;
using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun.Plan.v2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _svc;

        public CategoriesController(ICategoryService svc) => _svc = svc;

        [HttpPost]
        [Authorize] 
        [Route("AddCategory")]
        public async Task<IActionResult> Create([FromBody] CategoryComponentModel model)
        {
            try
            {
                var created = await _svc.CreateAsync(model);
                return CreatedAtAction(nameof(GetById),
                                       new { id = created.Id },
                                       created);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cat = await (_svc as dynamic).GetByIdAsync(id);
            return cat == null ? NotFound() : Ok(cat);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var list = await _svc.GetAllAsync();
            return Ok(list);
        }
    }
}
