using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Abstract;

namespace Data.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}