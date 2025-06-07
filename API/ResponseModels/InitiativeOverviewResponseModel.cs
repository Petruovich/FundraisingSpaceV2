using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class InitiativeOverviewResponseModel
    {
        public decimal TotalCollected { get; set; }
        public List<InitiativeMonthlyStatModel> Monthly { get; set; } = new();
    }
}
