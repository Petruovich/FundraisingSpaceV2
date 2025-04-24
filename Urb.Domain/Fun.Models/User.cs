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
    public class User
    {
            public Guid Id { get; set; }
            public string Email { get; set; }
            public string PasswordHash { get; set; }

            public ICollection<Initiative> CreatedInitiatives { get; set; }
            public ICollection<Donate> Donations { get; set; }       
    }
}
