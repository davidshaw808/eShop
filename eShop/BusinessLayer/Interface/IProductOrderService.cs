
using Common;

namespace BusinessLayer.Interface
{
    public interface IProductOrderService
    {
        (decimal RefundAmount, string Message)? UpdateAfterSale(Order o);
    }
}
