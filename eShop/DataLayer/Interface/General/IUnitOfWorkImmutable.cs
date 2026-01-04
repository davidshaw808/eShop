using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Interface.General
{
    public interface IUnitOfWorkImmutable<T>
    {
        Task<T?> GetInUoW(Guid key);
        void GenerateInUoW(T t);
    }
}
