using Common.Interface;
using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface IProductServiceAdmin : IGenerateUpdateDelete<Product>
    {
        bool IsValidTransaction(IEnumerable<Product> p);
    }
}
