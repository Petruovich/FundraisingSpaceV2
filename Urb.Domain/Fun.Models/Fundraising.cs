using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Domain.Fun.Models
{
    public class Fundraising
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime Deadline { get; set; }

        public Guid InitiativeId { get; set; }
        public Initiative Initiative { get; set; }

        public ICollection<Donate> Donations { get; set; }
    }
}
