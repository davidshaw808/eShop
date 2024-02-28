using BusinessLayer.Interface.User;
using Common;
using Common.Interface;

namespace BusinessLayer.Interface.Admin
{
    public interface ICustomerServiceAdmin: ICustomerService
    {
        public IEnumerable<Customer> GetAllCustomers();
        public IEnumerable<Customer> GetAllActiveCustomers();
    }
}
