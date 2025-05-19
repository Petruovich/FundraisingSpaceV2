using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Domain.Fun.Models
{
    public class FundraisingStat
    {
        public int Id { get; set; }
        public int FundraisingId { get; set; }
        public Fundraising Fundraising { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal Goal { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<FundraisingDailyIncome> DailyIncomes { get; set; }
    }
}
