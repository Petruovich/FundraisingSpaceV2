using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Domain.Fun.Models
{
    public class Initiative
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Fundraising> Fundraisings { get; set; } = new List<Fundraising>();
        public string ImageUrl { get; set; }
        public ICollection<Subscribe> Subscribes { get; set; }
        public InitiativeStat Stat { get; set; }
    }
}
