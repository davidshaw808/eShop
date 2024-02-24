using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface ICustomerDataAccess : IGenerateUpdateDelete<Customer>
    {
        public bool PermanentlyRemoveAllCustomerData(Guid altId);
        public Customer? Get(Guid altId);
        public IEnumerable<Customer> Get(Func<Customer, bool> filter);
    }
}
