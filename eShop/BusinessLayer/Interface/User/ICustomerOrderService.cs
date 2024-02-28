using Common;

namespace BusinessLayer.Interface.User
{
    public interface ICustomerOrderService
    {
        bool Generate(Customer t);
        bool Update(Customer t);
    }
}
