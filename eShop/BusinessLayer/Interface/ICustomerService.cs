using Common;
using Common.Interface;

namespace BusinessLayer.Interface
{
    public interface ICustomerService: IGenerateUpdateDelete<Customer>
    {
        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id);
        public bool RemoveAllCustomerInfo(Guid id);
        public bool AddItemsToBasket(Guid id, Product product);
        public IEnumerable<Customer> GetAllCustomers();
        public IEnumerable<Customer> GetAllActiveCustomers();
        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName);
        public IEnumerable<Customer> GetCustomerByEmail(string email);
        public IEnumerable<Address> GetCustomerAddresses(Guid altId);
    }
}
