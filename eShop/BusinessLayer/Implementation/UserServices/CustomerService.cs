using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User;

public class CustomerService(ICustomerDataAccess custDl) : ICustomerService
{
    readonly ICustomerDataAccess _customerDataAccess = custDl;

    public bool LogicalDelete(Customer t)
    {
        return _customerDataAccess.LogicalDelete(t);
    }

    public bool Generate(Customer t)
    {
        if (t == null || t.Email == null)
        {
            return false;
        }
        t.Id = null;
        t.Active = true;
        return _customerDataAccess.Generate(t);
    }

    public bool Update(Customer t)
    {
        if (t.Id == null || t.Key == null)
        {
            return false;
        }
        return _customerDataAccess.Update(t);
    }

    public IEnumerable<Customer> GetAllActiveCustomers()
    {
        return _customerDataAccess.Get(c => c.Active);
    }

    public IEnumerable<Customer> GetAllCustomers()
    {
        return _customerDataAccess.Get(c => true);
    }

    public IEnumerable<Customer> GetCustomerByEmail(string email)
    {
        return _customerDataAccess.Get(c => c.Email == email);
    }

    public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName)
    {
        return _customerDataAccess.Get(c => c.FirstName == firstName && c.LastName == lastName);
    }

    public IEnumerable<Order>? GetCustomerOrderHistory(Guid id)
    {
        return _customerDataAccess.Get(id)?.OrderHistory;
    }

    public IEnumerable<AddressInternal> GetCustomerAddresses(Guid Key)
    {
        return _customerDataAccess.Get(Key)?.OrderHistory?.Select(oh => oh.Address) ?? Enumerable.Empty<AddressInternal>();
    }

    public bool RequestRemoveAllCustomerData(Guid Key)
    {
        throw new NotImplementedException();
    }
}
