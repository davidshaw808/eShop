using Common;
using Common.Interface;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace DataLayer.Interface
{
    public interface IProductDataAccess : IGenerateUpdateDelete<Product>
    {
        public Product? Get(int id);
        public bool UpdateAll(IEnumerable<Product> products);
    }
}
