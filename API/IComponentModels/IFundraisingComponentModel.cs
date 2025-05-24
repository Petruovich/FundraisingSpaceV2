using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.IComponentModels
{
    public interface IFundraisingComponentModel
    {
        public string Title { get; set; }
        public decimal GoalAmount { get; set; }
        public DateTime Deadline { get; set; }
        public int InitiativeId { get; set; }
    }
}
