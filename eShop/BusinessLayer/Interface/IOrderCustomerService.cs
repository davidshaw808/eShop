using Common;

namespace BusinessLayer.Interface
{
    public interface IOrderCustomerService
    {
        bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer);
    }
}
