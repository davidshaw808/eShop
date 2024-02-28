using BusinessLayer.Interface.Admin;
using Common;
using DataLayer.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Implementation
{
    public class OrderCustomerService: IOrderCustomerServiceAdmin
    {
        readonly IOrderDataAccess _orderDataAccess;
        public OrderCustomerService(IOrderDataAccess orderDataAccess) 
        {
            this._orderDataAccess = orderDataAccess;
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
    }
}
