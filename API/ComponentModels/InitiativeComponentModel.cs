using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fun.Application.ComponentModels
{
    public class InitiativeComponentModel
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public string UserId { get; init; } = default!;
        public DateTime CreatedDate { get; init; }
        public IEnumerable<FundraisingComponentModel>? Fundraisings { get; init; }
    }
}
