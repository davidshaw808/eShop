using Common;
using Common.Interface;

namespace BusinessLayer.Interface.User
{
    public interface ICustomerService : IGenerateUpdateDelete<Customer>
    {
        public Task<IAsyncEnumerable<Order>?> GetCustomerOrderHistoryAsync(Guid id);
        public Task<bool> AddItemToBasketAsync(Guid customerAltId, Product product);
        public Task<IAsyncEnumerable<Customer>> GetCustomerByNameAsync(string firstName, string lastName);
        public Task<IAsyncEnumerable<Customer>> GetCustomerByEmailAsync(string email);
        public Task<IAsyncEnumerable<Address>> GetCustomerAddressesAsync(Guid altId);
        public Task<bool> RequestRemoveAllCustomerDataAsync(Guid altId);
    }
}
