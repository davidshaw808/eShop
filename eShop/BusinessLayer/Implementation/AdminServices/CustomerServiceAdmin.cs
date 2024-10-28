using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin;

public class CustomerServiceAdmin(IOrderCustomerServiceAdmin orderCustomerServiceAdmin, ICustomerDataAccess customerDataAccess): 
    CustomerService(customerDataAccess), ICustomerServiceAdmin
{
    private readonly ICustomerDataAccess _customerDataAccess = customerDataAccess;
    private readonly IOrderCustomerServiceAdmin _orderCustomerServiceAdmin = orderCustomerServiceAdmin;

    public IAsyncEnumerable<Customer> GetCustomersRequestingRemovalAsync()
    {
        return _customerDataAccess.GetAsync(c => c.RemoveAllCustomerDataRequest != null);
    }

    public async ValueTask<bool> RemoveAllCustomerInfoAsync(Guid id, string email)
    {
        var deletedCustomer = await _customerDataAccess.GetAsync(id);
        if (deletedCustomer == null || deletedCustomer.Email != email || deletedCustomer.RemoveAllCustomerDataRequest == null)
        {
            return false;//possible malicious query
        }
        if (deletedCustomer.OrderHistory?.Any(o => o.Updates?.Any(u => u.Status == OrderStatus.Shipped || u.Status == OrderStatus.InTransit || u.Status == OrderStatus.Paid) ?? false) ?? false)
        {
            throw new InvalidOperationException("Customer has items yet to be delivered");
        }
        var dummyCustomer = GenerateDummyCustomer("Removed@Customer.com");
        Generate(dummyCustomer);
        dummyCustomer.Key ??= Guid.NewGuid();
        _orderCustomerServiceAdmin.TransferOrderHistory(id, dummyCustomer);
        return _customerDataAccess.PermanentlyRemoveCustomer(id);
    }

    private Customer GenerateDummyCustomer(string email)
    {
        return new Customer()
        {
            FirstName = "User_",
            LastName = Guid.NewGuid().ToString(),
            Active = false,
            Email = email,
            Address = null,
            Basket = null
        };
    }

    Task<IAsyncEnumerable<Customer>> ICustomerServiceAdmin.GetAllActiveCustomers()
    {
        throw new NotImplementedException();
    }

    Task<IAsyncEnumerable<Customer>> ICustomerServiceAdmin.GetAllCustomersAsync()
    {
        throw new NotImplementedException();
    }

    Task<IAsyncEnumerable<Customer>> ICustomerServiceAdmin.GetCustomersRequestingRemoval()
    {
        throw new NotImplementedException();
    }

    Task<bool> ICustomerServiceAdmin.RemoveAllCustomerInfoAsync(Guid id, string email)
    {
        throw new NotImplementedException();
    }
}

