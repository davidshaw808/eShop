using BusinessLayer.Interface.User;
using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface ICustomerOrderServiceAdmin: ICustomerOrderService
    {
        bool AddOrder(Guid id, Order order);
    }
}
