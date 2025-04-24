using System.ComponentModel.DataAnnotations;

namespace Urb.Plan.v2.Views
{
    public class AuthenticateRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
