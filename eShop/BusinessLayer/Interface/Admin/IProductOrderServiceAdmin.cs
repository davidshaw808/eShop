using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface IProductOrderServiceAdmin
    {
        (decimal RefundAmount, string Message)? UpdateAfterSale(Order o);
    }
}
