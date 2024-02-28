using BusinessLayer.Implementation;
using Common;

namespace BusinessLayer.Interface.Admin
{
    public interface IOrderCustomerServiceAdmin
    {
        bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer);
    }
}
