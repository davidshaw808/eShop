using BusinessLayer.Interface;
using Common;
using DataLayer.Interface;


namespace BusinessLayer.Implementation
{
    public class CustomerOrderService: ICustomerOrderService
    {
        readonly ICustomerDataAccess _customerDataAccess;

        public CustomerOrderService(ICustomerDataAccess custDl)
        {
            this._customerDataAccess = custDl;
        }

        public bool AddOrder(Guid id, Order order)
        {
            if (order.Id == null || order.AltId == null)
            {
                return false;
            }
            var customer = this._customerDataAccess.Get(id);
            if (customer == null)
            {
                return false;
            }
            if (customer.OrderHistory == null)
            {
                customer.OrderHistory = new List<Order>();
            }
            customer.OrderHistory.Add(order);
            return true;
        }

        public bool Generate(Customer t)
        {
            if (t == null || t.Email == null)
            {
                return false;
            }
            t.Id = null;
            t.Active = true;
            return this._customerDataAccess.Generate(t);
        }

        public bool Update(Customer t)
        {
            if (t.Id == null || t.AltId == null)
            {
                return false;
            }
            return this._customerDataAccess.Update(t);
        }
    }
}
