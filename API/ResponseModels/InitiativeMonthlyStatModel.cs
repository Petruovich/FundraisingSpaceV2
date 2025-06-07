using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class InitiativeMonthlyStatModel
    {
        public string YearMonth { get; set; } = default!;
        public decimal TotalAmount { get; set; }
    }
}
