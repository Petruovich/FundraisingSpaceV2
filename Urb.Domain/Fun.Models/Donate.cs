using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Domain.Fun.Models
{
    public class Donate
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid FoundrassingId { get; set; }
        public Fundraising Foundrassing { get; set; }
    }
}
