using Common;
using Common.Interface;

namespace BusinessLayer.Interface.User
{
    public interface ICustomerService : IGenerateUpdateDelete<Customer>
    {
        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id);
        public bool RemoveAllCustomerInfo(Guid id, string email);
        public bool AddItemsToBasket(Guid id, Product product);
        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName);
        public IEnumerable<Customer> GetCustomerByEmail(string email);
        public IEnumerable<Address> GetCustomerAddresses(Guid altId);
    }
}
