using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class FundraisingResponseTotalCollectedModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public decimal GoalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime Deadline { get; set; }
        public decimal TotalCollected { get; set; }
    }
}
