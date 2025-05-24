using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Application.TestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonateController : ControllerBase
    {
        private readonly IDonateService _donationSvc;
        private readonly IHttpContextAccessor _httpCtx;
        public DonateController(IDonateService donationSvc, IHttpContextAccessor httpCtx)
        {
            _donationSvc = donationSvc;
            _httpCtx = httpCtx;
        }

        [HttpPost("{id}/donate")]
        [Authorize]
        public async Task<IActionResult> Donate(int fundraisingId, [FromBody] DonateComponentModel donateComponentModel)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _donationSvc.DonateAsync(fundraisingId, donateComponentModel.Amount, userId);
            return Ok();
        }

        [Route("DonateTest")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TestDonateComponentModel dto)
        {
            await _donationSvc.DonateAsync(dto.FundraisingId, dto.Amount, dto.UserId);
            return Ok(new
            {
                dto.FundraisingId,
                dto.Amount,
                dto.UserId
            });
        }

    }
}
