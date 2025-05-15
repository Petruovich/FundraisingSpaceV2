using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Domain.Fun.Models
{
        public class Role : IdentityRole<int>
        {
            public ICollection<IdentityUserRole<int>> UserRoles { get; set; }
        }
}
