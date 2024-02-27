using Common;

namespace BusinessLayer.Interface
{
    public interface ICustomerOrderService
    {
        bool AddOrder(Guid id, Order order);
        bool Generate(Customer t);
        bool Update(Customer t);
    }
}
