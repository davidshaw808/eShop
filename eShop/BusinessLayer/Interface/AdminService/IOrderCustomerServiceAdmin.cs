using BusinessLayer.Implementation;
using Common.Models.Mutable;

namespace BusinessLayer.Interface.Admin;

public interface IOrderCustomerServiceAdmin
{
    bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer);
}
