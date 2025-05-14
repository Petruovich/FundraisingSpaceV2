using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Domain.Fun.Models
{
    public class InitiativeStat
    {
        public int Id { get; set; }
        public int InitiativeId { get; set; }
        public Initiative Initiative { get; set; }
        public int TotalViews { get; set; }
        public int TotalFundraisings { get; set; }
        public int TotalSubscribers { get; set; }
        public DateTime LastActivityAt { get; set; } = DateTime.MinValue;
    }
}
