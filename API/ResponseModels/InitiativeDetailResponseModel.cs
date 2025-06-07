using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ResponseModels
{
    public class InitiativeDetailResponseModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        public string? ImageBase64 { get; set; }
        //public List<FundraisingResponseModel> Fundraisings { get; set; } = new();
    }
}
