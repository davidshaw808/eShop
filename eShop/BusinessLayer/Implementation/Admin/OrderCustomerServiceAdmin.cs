using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.Admin
{
    public class OrderCustomerServiceAdmin(IOrderDataAccess orderDataAccess) : IOrderCustomerServiceAdmin
    {
        readonly IOrderDataAccess _orderDataAccess = orderDataAccess;

        public bool TransferOrderHistory(Guid currentCustomer, Customer newCustomer)
        {
            var orders = _orderDataAccess.Get(o => o.Customer.AltId == currentCustomer);
            if (orders == null)
            {
                return false;
            }
            foreach (var order in orders)
            {
                order.Customer = newCustomer;
            }
            return _orderDataAccess.Update(orders);
        }
    }
}
