using BusinessLayer.ClassHelpers;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User
{
    public class OrderService(IOrderDataAccess orderDataAccess, ICustomerOrderService customerOrderService) : IOrderService
    {
        readonly IOrderDataAccess _orderDataAccess = orderDataAccess;
        readonly ICustomerOrderService _customerOrderService = customerOrderService;

        public bool AddOrderUpdate(Guid orderId, OrderUpdate update)
        {
            var order = _orderDataAccess.Get(orderId);
            return AddOrderUpdate(order, update);
        }

        public bool AddOrderUpdate(Order order, OrderUpdate orderUpdate)
        {
            if (order == null || order.Delivered)
            {
                return false;
            }
            orderUpdate.Order = order;
            order.Updates ??= [];
            if (orderUpdate.CreatedDate == DateTime.MinValue)
            {
                orderUpdate.CreatedDate = DateTime.UtcNow;
            }
            order.Updates.Add(orderUpdate);
            if (orderUpdate.Status == OrderStatus.Delivered)
            {
                order.Delivered = true;
            }
            else if (orderUpdate.Status == OrderStatus.Cancelled)
            {
                order.Cancelled = true;
            }
            return _orderDataAccess.Update(order);
        }

        public Order? GetOrder(Guid orderId)
        {
            return _orderDataAccess.Get(orderId);
        }

        public Error CanProcessBasket(Guid AltCustId)
        {
            var result = this._customerOrderService.CanProcessBasket(AltCustId);
            return new Error { IsError = !result.CanProcess, Message = result.Message };
        }

        public decimal PreparePayment(Guid AltCustId)
        {
            var result = this._customerOrderService.PrepareBasketPayment(AltCustId);
            if(result == null || !result.Any())
            {
                this._customerOrderService.ClearBasket(AltCustId);
                return 0;
            }
            return result.Sum(p => p.Price);
        }
    }
}
