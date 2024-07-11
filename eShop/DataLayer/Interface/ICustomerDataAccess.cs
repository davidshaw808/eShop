using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface ICustomerDataAccess : IGenerateUpdateDelete<Customer>
    {
        bool PermanentlyRemoveCustomer(Guid altId);
        Customer? Get(Guid altId);
        IEnumerable<Customer?> Get(Func<Customer, bool> filter);
        IAsyncEnumerable<Customer> GetAsync(Func<Customer, bool> filter);
    }
}
