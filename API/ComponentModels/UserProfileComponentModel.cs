using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class UserProfileComponentModel
    {
        public string? FirstName { get; set; }

        public string? SecondName { get; set; }
     
        [EmailAddress]
        public string? Email { get; set; }

        public IFormFile? AvatarFile { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
