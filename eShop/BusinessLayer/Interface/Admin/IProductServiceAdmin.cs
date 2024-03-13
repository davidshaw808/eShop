using Common.Interface;
using Common;
using BusinessLayer.Interface.User;

namespace BusinessLayer.Interface.Admin
{
    public interface IProductServiceAdmin : IGenerateUpdateDelete<Product>, IProductService
    {
        bool IsValidTransaction(IEnumerable<Product> p);
    }
}
 