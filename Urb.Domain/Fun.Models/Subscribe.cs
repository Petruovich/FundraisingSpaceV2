using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Domain.Fun.Models
{
    public class Subscribe
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int InitiativeId { get; set; }
        public Initiative Initiative { get; set; }
        public DateTime SubscribedAt { get; set; }
    }
}
