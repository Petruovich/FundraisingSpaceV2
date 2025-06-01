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
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}
