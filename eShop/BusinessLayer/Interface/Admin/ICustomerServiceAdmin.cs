using BusinessLayer.Interface.User;
using Common;
using Common.Interface;

namespace BusinessLayer.Interface.Admin
{
    public interface ICustomerServiceAdmin: ICustomerService
    {
        public IEnumerable<Customer> GetAllCustomers();
        public IEnumerable<Customer> GetAllActiveCustomers();
        public IEnumerable<Customer> GetCustomersRequestingRemoval();
        public bool RemoveAllCustomerInfo(Guid id, string email);
    }
}
