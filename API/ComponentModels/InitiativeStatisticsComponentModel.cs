using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class InitiativeStatisticsComponentModel
    {
        public int InitiativeId { get; set; }
        public string Title { get; set; } = null!;
        public int TotalViews { get; set; }
        public int TotalFundraisings { get; set; }
        public int TotalSubscribers { get; set; }
        public List<FundraisingStatisticsComponentModel> Fundraisings { get; set; } = new();
    }
}
