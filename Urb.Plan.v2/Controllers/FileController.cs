using Fun.Application.Fun.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Fun.Plan.v2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _files;

        public FileController(IFileService files)
        {
            _files = files;
        }
          
        [HttpGet("{*relativePath}")]
        public async Task<IActionResult> Get(string relativePath)
        {
            try
            {
                using var stream = await _files.OpenReadAsync(relativePath);

                var ext = Path.GetExtension(relativePath).ToLowerInvariant();
                var contentType = ext switch
                {
                    ".png" => "image/png",
                    ".jpg" => "image/jpeg",
                    ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };
                return File(stream, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
