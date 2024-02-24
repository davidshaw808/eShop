using Common.Interface;
using Common;

namespace BusinessLayer.Interface
{
    public interface IProductService : IGenerateUpdateDelete<Product>
    {
        (decimal RefundAmount, string Message)? UpdateAfterSale(Order o);
        bool IsValidTransaction(IEnumerable<Product> p);
    }
}
