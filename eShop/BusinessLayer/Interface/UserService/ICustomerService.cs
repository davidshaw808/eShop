using Common;
using Common.Interface;

namespace BusinessLayer.Interface.User;

public interface ICustomerService : IGenerateUpdateDelete<Customer>
{
    
    public Task<bool> AddItemToBasketAsync(Guid customerKey, Product product);
    public Task<IAsyncEnumerable<Customer>> GetCustomerByNameAsync(string firstName, string lastName);
    public Task<IAsyncEnumerable<Customer>> GetCustomerByEmailAsync(string email);
    public Task<IAsyncEnumerable<AddressInternal>> GetCustomerAddressesAsync(Guid Key);
    public Task<bool> RequestRemoveAllCustomerDataAsync(Guid Key);
}
