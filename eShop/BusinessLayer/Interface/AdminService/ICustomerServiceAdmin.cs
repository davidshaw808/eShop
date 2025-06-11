using BusinessLayer.Interface.User;
using Common.Interface;
using Common.Models.Mutable;

namespace BusinessLayer.Interface.Admin;

public interface ICustomerServiceAdmin: ICustomerService
{
    public Task<IAsyncEnumerable<Customer>> GetAllCustomersAsync();
    public Task<IAsyncEnumerable<Customer>> GetAllActiveCustomers();
    public Task<IAsyncEnumerable<Customer>> GetCustomersRequestingRemoval();
    public Task<bool> RemoveAllCustomerInfoAsync(Guid id, string email);
}
