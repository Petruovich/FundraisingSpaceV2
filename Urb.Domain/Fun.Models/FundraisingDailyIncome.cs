using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Domain.Fun.Models
{
    public class FundraisingDailyIncome
    {
        public int Id { get; set; }
        public int FundraisingStatId { get; set; }
        public FundraisingStat FundraisingStat { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
