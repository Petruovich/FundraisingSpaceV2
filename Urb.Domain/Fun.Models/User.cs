using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urb.Domain.Urb.Models
{
    public class User: IdentityUser<int> 
    {
            public string AvatarUrl { get; set; } = string.Empty;
            public ICollection<Subscribe> Subscribes { get; set; }
            public ICollection<Initiative> Initiatives { get; set; }
            public ICollection<Fundraising> Fundraisings { get; set; }
            public ICollection<Donate> Donates { get; set; }
    }
}
