using BusinessLayer.Interface;
using Common;
using Common.Enum;
using DataLayer.Implementation;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class CustomerService : ICustomerService
    {
        readonly ICustomerDataAccess _customerDataAccess;
        readonly IOrderDataAccess _orderDataAccess;

        public CustomerService(ICustomerDataAccess custDl, IOrderDataAccess orderService)
        {
            this._customerDataAccess = custDl;
            this._orderDataAccess = orderService;
        }

        public bool AddOrder(Guid id, Order order)
        {
            if(order.Id == null || order.AltId == null)
            {
                return false;
            }
            var customer = this._customerDataAccess.Get(id);
            if (customer == null)
            {
                return false;
            }
            if(customer.OrderHistory == null)
            {
                customer.OrderHistory = new List<Order>();
            }
            customer.OrderHistory.Add(order);
            return true;
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

        public bool Generate(Customer t)
        {
            if(t == null || t.Email == null)
            {
                return false;
            }
            t.Id = null;
            t.Active = true;
            return this._customerDataAccess.Generate(t);
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
            if(deletedCustomer.OrderHistory?.Any(o => o.Updates?.Any(u => u.Status != OrderStatus.Delivered) ?? true) ?? false)
            {
                throw new InvalidOperationException("Customer has items yet to be delivered");
            }
            var dummyCustomer = new Customer()
            {
                FirstName = "User_",
                LastName = Guid.NewGuid().ToString(),
                Active = false,
                Email = "RemovedCustomer",
                Address = null,
                Basket = null
            };
            this.Generate(dummyCustomer);
            if(dummyCustomer.AltId == null)
            {
                dummyCustomer.AltId = Guid.NewGuid();
            }
            this.TransferOrderHistory(id, dummyCustomer);
            return this._customerDataAccess.PermanentlyRemoveAllCustomerData(id);
        }

        public bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer)
        {
            var orders = this._orderDataAccess.Get(o => o.Customer.AltId == currentCustomer);
            if (orders == null)
            {
                return false;
            }
            foreach (var order in orders)
            {
                order.Customer = newCustomer;
            }
            return this._orderDataAccess.Update(orders);
        }

        public bool Update(Customer t)
        {
            if(t.Id == null || t.AltId == null)
            {
                return false;
            }
            return this._customerDataAccess.Update(t);
        }

        public IEnumerable<Address>? GetCustomerAddresses(Guid altId)
        {
            return this._customerDataAccess.Get(altId)?.OrderHistory?.Select(oh => oh.Address);
        }
    }
}
