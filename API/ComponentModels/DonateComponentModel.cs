using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class DonateComponentModel
    {
        //public string Id { get; init; } = default!;
        //public string UserId { get; init; } = default!;
        //public string FundraisingId { get; init; } = default!;
        //public decimal Amount { get; init; }
        //public DateTime Date { get; init; }
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
