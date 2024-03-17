using Common;
using Common.Interface;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace DataLayer.Interface
{
    public interface IProductDataAccess : IGenerateUpdateDelete<Product>
    {
        public Product? Get(Guid AltId);
        public IEnumerable<Product>? GetAll(IEnumerable<Guid> AltIds);
        public bool UpdateAll(IEnumerable<Product> products);
    }
}
