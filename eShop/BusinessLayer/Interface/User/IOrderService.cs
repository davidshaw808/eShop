using BusinessLayer.ClassHelpers;
using Common;

namespace BusinessLayer.Interface.User
{
    public interface IOrderService
    {
        bool AddOrderUpdate(Guid orderId, OrderUpdate update);
        Order? GetOrder(Guid orderId);
        bool AddOrderUpdate(Order order, OrderUpdate orderUpdate);
        Error CanProcessBasket(Guid AltCustId);
        decimal PreparePayment(Guid AltCustId);
    }
}
