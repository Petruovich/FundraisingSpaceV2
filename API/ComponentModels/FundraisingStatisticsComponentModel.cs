using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class FundraisingStatisticsComponentModel
    {
        public int FundraisingId { get; set; }
        public decimal Goal { get; set; }
        public decimal TotalCollected { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<FundraisingDailyIncomeComponentModel> DailyIncomes { get; set; } = new();
    }
}
