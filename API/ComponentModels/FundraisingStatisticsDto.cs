using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class FundraisingStatisticsDto
    {
        public int FundraisingId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Goal { get; set; }
        public decimal TotalCollected { get; set; }
        public List<DailyIncomeDto> DailyIncomes { get; set; } = new();
    }
}
