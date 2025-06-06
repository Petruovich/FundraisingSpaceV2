using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class DonorResponseModel
    {
        public int UserId { get; set; }
    
        public string NormalizedName { get; set; } = default!;

        public string? AvatarBase64 { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
