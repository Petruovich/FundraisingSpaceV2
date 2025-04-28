using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Urb.Application.Urb.IUnitOfWork
{
    public interface IUnitOfWOrk
    {
        public void BeginTransaction();
        public int Commit();
        public void Rollback();

    }
}
