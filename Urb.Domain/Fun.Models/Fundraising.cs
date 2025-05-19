using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Domain.Fun.Models
{
    public class Fundraising
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public int CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public int InitiativeId { get; set; }
        public Initiative Initiative { get; set; }
        public FundraisingStat Stat { get; set; }
        public ICollection<Donate> Donates { get; set; }
    }
}
