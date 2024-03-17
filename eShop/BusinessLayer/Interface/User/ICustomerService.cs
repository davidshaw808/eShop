using Common;
using Common.Interface;

namespace BusinessLayer.Interface.User
{
    public interface ICustomerService : IGenerateUpdateDelete<Customer>
    {
        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id);
        public bool AddItemToBasket(Guid customerAltId, Product product);
        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName);
        public IEnumerable<Customer> GetCustomerByEmail(string email);
        public IEnumerable<Address> GetCustomerAddresses(Guid altId);
        public bool RequestRemoveAllCustomerData(Guid altId);
    }
}
