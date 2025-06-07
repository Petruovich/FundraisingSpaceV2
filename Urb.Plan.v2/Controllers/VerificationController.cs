using Fun.Application.Fun.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verif;

        public VerificationController(IVerificationService verif)
            => _verif = verif;

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            var ok = await _verif.ConfirmEmailAsync(token);
            if (!ok) return BadRequest("Невірний або прострочений токен.");
            return Ok("Email підтверджено.");
        }

        [HttpPost("verify-2fa")]
        public async Task<IActionResult> VerifyTwoFactor([FromBody] TwoFaVerifyModel model)
        {
            var ok = await _verif.VerifyTwoFactorAsync(model.Code);
            if (!ok) return BadRequest("Невірний 2FA код.");
            return Ok("2FA пройдена.");
        }
    }
    public class TwoFaVerifyModel
    {
        public string Code { get; set; }
    }
}
