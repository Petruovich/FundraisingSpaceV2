using Fun.Application.ComponentModels;
using Fun.Application.Fun.IServices;
using Fun.Infrastructure.Fun.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;

namespace Fun.Plan.v2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeController : ControllerBase
    {
        private readonly IStripeService _stripeSvc;
        private readonly IUserService _userService;

        public StripeController(IStripeService stripeSvc, IUserService userService)
        {
            _stripeSvc = stripeSvc; 
            _userService = userService;
        }
             
            

        [HttpPost("create-checkout-session")]
        [Authorize]
        public async Task<IActionResult> CreateCheckoutSession(
            [FromBody] CreateSessionComponentModel dto)
        {
            var userId = await _userService.GetMy();   /*int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value)*/;

            var sessionId = await _stripeSvc.CreateCheckoutSessionAsync(
                dto.FundraisingId,
                dto.Amount,
                dto.SuccessUrl,
                dto.CancelUrl,
                userId);

            return Ok(new { sessionId });
        }

        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(Request.Body).ReadToEndAsync();
            var sig = Request.Headers["Stripe-Signature"];

            try
            {
                await (_stripeSvc as StripeService)!
                    .HandleWebhookAsync(json, sig);
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
        }


        
    }
}
