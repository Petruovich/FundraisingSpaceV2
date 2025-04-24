using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urb.Domain.Urb.Models;

namespace Urb.Application.Urb.IRepositories
{
    public interface IUserRepository
    {
        public interface IUserRepository
        {
            public void Create(User user);
            public User GetById(string id);
            public int SaveChanges();
            public void Update(User user);

        }
    }
}
