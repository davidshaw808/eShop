using Common;
using Common.Models.Mutable;
using DataLayer.Interface.General;
using System.Threading.Tasks;

namespace DataLayer.Interface;

public interface ICustomerDataAccess : IUnitOfWorkCRUD<Customer>, IAtomicCRUD<Customer>
{
    Task<bool> PermanentlyRemoveCustomerDetailsAsync(Customer customer);
    Task<Customer?> GetAtomicAsync(string email);
    IEnumerable<Customer?> Get(Func<Customer, bool> filter);
    IAsyncEnumerable<Customer> GetAsync(Func<Customer, bool> filter);
}
