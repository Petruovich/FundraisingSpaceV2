using Fun.Application.IComponentModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class InitiativeComponentModel : IInitiativeComponentModel
    {
        //public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; }
        //public string CategoryName { get; set; } = default!;
        public IFormFile? ImageFile { get; set; }
        //public int CreatedById { get; set; }
    }
}
