using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class CreateSessionComponentModel
    {
        public int FundraisingId { get; set; }
        public decimal Amount { get; set; }
        public string SuccessUrl { get; set; } = null!;
        public string CancelUrl { get; set; } = null!;
    }
}
