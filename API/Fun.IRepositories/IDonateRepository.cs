using Fun.Domain.Fun.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Fun.Application.Fun.IRepositories
{
    public interface IDonateRepository
    {
        public void Create(User user);
        public Donate GetById(string id);
        public int SaveChanges();
        public void Update(User user);
    }
}
