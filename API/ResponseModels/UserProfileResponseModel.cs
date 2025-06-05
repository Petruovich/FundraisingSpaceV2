using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class UserProfileResponseModel
    {
        public string FirstName { get; set; } = default!;

        public string SecondName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string? ImageBase64 { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
