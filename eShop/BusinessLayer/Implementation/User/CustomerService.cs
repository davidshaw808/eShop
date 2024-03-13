using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User
{
    public class CustomerService : CustomerOrderService, ICustomerServiceAdmin, ICustomerService
    {
        readonly ICustomerDataAccess _customerDataAccess;
        readonly IOrderCustomerServiceAdmin _orderCustomerService;

        public CustomerService(ICustomerDataAccess custDl, IOrderCustomerServiceAdmin orderCustomerService) : base(custDl)
        {
            _customerDataAccess = custDl;
            _orderCustomerService = orderCustomerService;
        }

        public bool AddItemsToBasket(Guid id, Product product)
        {
            if (product.Id == null)
            {
                return false;
            }
            var customer = _customerDataAccess.Get(id);
            if (customer == null)
            {
                return false;
            }
            if (customer.Basket == null)
            {
                customer.Basket = new List<Product>();
            }
            customer.Basket.Add(product);
            return true;
        }

        public bool Delete(Customer t)
        {
            return _customerDataAccess.Delete(t);
        }

        public IEnumerable<Customer> GetAllActiveCustomers()
        {
            return _customerDataAccess.Get(c => c.Active);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return _customerDataAccess.Get(c => true);
        }

        public IEnumerable<Customer> GetCustomerByEmail(string email)
        {
            return _customerDataAccess.Get(c => c.Email == email);
        }

        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName)
        {
            return _customerDataAccess.Get(c => c.FirstName == firstName && c.LastName == lastName);
        }

        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id)
        {
            return _customerDataAccess.Get(id)?.OrderHistory;
        }

        public bool RemoveAllCustomerInfo(Guid id, string email)
        {
            var deletedCustomer = _customerDataAccess.Get(id);
            if (deletedCustomer == null || deletedCustomer.Email != email)
            {
                return false;//possible malicious query
            }
            if (deletedCustomer.OrderHistory?.Any(o => o.Updates?.Any(u => u.Status == OrderStatus.Shipped || u.Status == OrderStatus.InTransit || u.Status == OrderStatus.Paid) ?? true) ?? false)
            {
                throw new InvalidOperationException("Customer has items yet to be delivered");
            }
            var dummyCustomer = GenerateDummyCustomer("RemovedCustomer");
            Generate(dummyCustomer);
            if (dummyCustomer.AltId == null)
            {
                dummyCustomer.AltId = Guid.NewGuid();
            }
            _orderCustomerService.TransferOrderHistory(id, dummyCustomer);
            return _customerDataAccess.PermanentlyRemoveAllCustomerData(id);
        }

        public IEnumerable<Address>? GetCustomerAddresses(Guid altId)
        {
            return _customerDataAccess.Get(altId)?.OrderHistory?.Select(oh => oh.Address);
        }

        private Customer GenerateDummyCustomer(string email)
        {
            return new Customer()
            {
                FirstName = "User_",
                LastName = Guid.NewGuid().ToString(),
                Active = false,
                Email = email,
                Address = null,
                Basket = null
            };
        }
    }
}
