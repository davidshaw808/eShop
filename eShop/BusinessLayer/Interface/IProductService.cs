using Common.Interface;
using Common;

namespace BusinessLayer.Interface
{
    public interface IProductService : IGenerateUpdateDelete<Product>
    {
        bool IsValidTransaction(IEnumerable<Product> p);
    }
}
