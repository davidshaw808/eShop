using BusinessLayer.Interface;
using Common;
using Common.Enum;  
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class CustomerService : CustomerOrderService, ICustomerService
    {
        readonly ICustomerDataAccess _customerDataAccess;
        readonly IOrderCustomerService _orderCustomerService;

        public CustomerService(ICustomerDataAccess custDl, IOrderCustomerService orderCustomerService): base(custDl)
        {
            this._customerDataAccess = custDl;
            this._orderCustomerService = orderCustomerService;
        }

        public bool AddItemsToBasket(Guid id, Product product)
        {
            if (product.Id == null)
            {
                return false;
            }
            var customer = this._customerDataAccess.Get(id);
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
            return this._customerDataAccess.Delete(t);
        }

        public IEnumerable<Customer> GetAllActiveCustomers()
        {
            return this._customerDataAccess.Get(c => c.Active);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return this._customerDataAccess.Get(c => true);
        }

        public IEnumerable<Customer> GetCustomerByEmail(string email)
        {
            return this._customerDataAccess.Get(c => c.Email == email);
        }

        public IEnumerable<Customer> GetCustomerByName(string firstName, string lastName)
        {
            return this._customerDataAccess.Get(c => c.FirstName == firstName && c.LastName == lastName);
        }

        public IEnumerable<Order>? GetCustomerOrderHistory(Guid id)
        {
            return this._customerDataAccess.Get(id)?.OrderHistory;
        }

        public bool RemoveAllCustomerInfo(Guid id)
        {
            var deletedCustomer = this._customerDataAccess.Get(id);
            if(deletedCustomer == null)
            {
                return false;//possible malicious query
            }
            if(deletedCustomer.OrderHistory?.Any(o => o.Updates?.Any(u => u.Status == OrderStatus.Shipped || u.Status == OrderStatus.InTransit || u.Status == OrderStatus.Paid) ?? true) ?? false)
            {
                throw new InvalidOperationException("Customer has items yet to be delivered");
            }
            var dummyCustomer = GenerateDummyCustomer("RemovedCustomer");
            this.Generate(dummyCustomer);
            if(dummyCustomer.AltId == null)
            {
                dummyCustomer.AltId = Guid.NewGuid();
            }
            this._orderCustomerService.TransferOrderHistory(id, dummyCustomer);
            return this._customerDataAccess.PermanentlyRemoveAllCustomerData(id);
        }

        public IEnumerable<Address>? GetCustomerAddresses(Guid altId)
        {
            return this._customerDataAccess.Get(altId)?.OrderHistory?.Select(oh => oh.Address);
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
