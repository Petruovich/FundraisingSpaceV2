using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class DonateResponseModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string FundraisingTitle { get; set; } = default!;
        public DateTime FundraisingDeadline { get; set; }
        public decimal FundraisingGoal { get; set; }
        public int InitiativeCategoryId { get; set; }
        public string InitiativeTitle { get; set; } = default!;
    }
}
