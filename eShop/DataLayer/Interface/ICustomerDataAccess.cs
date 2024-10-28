using Common;
using Common.Interface;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface ICustomerDataAccess : IUnitOfWorkCUD<Customer>, IAtomicCRUD<Customer>
{
    Task<int> PermanentlyRemoveCustomer(Guid Key);
    Task<Customer?> GetAsync(Guid Key);
    IEnumerable<Customer?> Get(Func<Customer, bool> filter);
    IAsyncEnumerable<Customer> GetAsync(Func<Customer, bool> filter);
}
