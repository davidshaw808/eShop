using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using DataLayer.Interface;


namespace BusinessLayer.Implementation.Admin
{
    public class CustomerOrderServiceAdmin(ICustomerDataAccess custDl) : CustomerOrderService(custDl), ICustomerOrderServiceAdmin
    {
        readonly ICustomerDataAccess _customerDataAccess = custDl;

        public bool AddOrder(Guid id, Order order)
        {
            if (order.Id == null || order.AltId == null)
            {
                return false;
            }
            var customer = _customerDataAccess.Get(id);
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
    }
}
