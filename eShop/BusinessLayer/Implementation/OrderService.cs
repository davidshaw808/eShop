using BusinessLayer.Implementation.Admin;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class OrderService(IOrderDataAccess orderDataAccess,
        IRefundDataAccess refundDataAccess,
        IAddressOrderService addressOrderService,
        ICustomerOrderServiceAdmin customerOrderService,
        IProductOrderServiceAdmin productOrderService) : OrderCustomerServiceAdmin(orderDataAccess), IOrderService
    {
        readonly IOrderDataAccess _orderDataAccess = orderDataAccess;
        readonly IRefundDataAccess _refundDataAccess = refundDataAccess;
        readonly IAddressOrderService _addressOrderService = addressOrderService;
        readonly ICustomerOrderServiceAdmin _customerOrderService = customerOrderService;
        readonly IProductOrderServiceAdmin _productOrderService = productOrderService;

        public bool AddOrderUpdate(Guid orderId, OrderUpdate update)
        {
            var order = this._orderDataAccess.Get(orderId);
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
            if(orderUpdate.CreatedDate == DateTime.MinValue)
            {
                orderUpdate.CreatedDate = DateTime.UtcNow;
            }
            order.Updates.Add(orderUpdate);
            if (orderUpdate.Status == OrderStatus.Delivered)
            {
                order.Delivered = true;
            }
            else if(orderUpdate.Status == OrderStatus.Cancelled)
            {
                order.Cancelled = true;
            }
            return this._orderDataAccess.Update(order);
        }

        public bool Delete(Order t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            return this._orderDataAccess.Delete(t);
        }

        public bool Generate(Order order)
        {
            if (order.AltId != null || order.PaymentDetails == null)
            {
                return false;
            }
            this._orderDataAccess.Generate(order);
            if(order.Customer.AltId == null)
            {
                this._customerOrderService.Generate(order.Customer);
                  this._customerOrderService.AddOrder((Guid)order.Customer.AltId, order);
            }
            this.AddOrderUpdate(order, OrderStatus.Paid, "Order generated");
            if (order.Address.AltId == null)
            {
                this._addressOrderService.Generate(order.Address);
            }
            if(order.PaymentDetails.Id == null)
            {
                this._orderDataAccess.Generate(order.PaymentDetails);
            }
            order.Active = true;
            var result = this._productOrderService.UpdateAfterSale(order);
            if (result != null)
            {
                //log here
                GenerateRefund(order, result.Value.RefundAmount, result.Value.Message);
            }
            return true;
        }

        public bool Update(Order order)
        {
            if (order.Id == null || order.AltId == null)
            {
                return false;
            }
            return this._orderDataAccess.Update(order);
        }

        public Order? GetOrder(Guid orderId)
        {
            return this._orderDataAccess.Get(orderId);
        }

        protected bool AddOrderUpdate(Order order, OrderStatus orderStatus, string text)
        {
            var ou = new OrderUpdate()
            {
                Order = order,
                Status = orderStatus,
                UpdateText = text
            };
            if(order.AltId == null)
            {
                if (!this.Generate(order))
                {
                    return false;
                }
            }
            return this.AddOrderUpdate((Guid)order.AltId, ou);
        }

        protected Refund GenerateRefund(Order order, decimal amount, string description)
        {
            var refund = new Refund()
            {
                Order = order,
                DescriptionOfRefund = description,
                Amount = amount,
                DateGenerated = DateTime.UtcNow,
            };
            this._refundDataAccess.Generate(refund);
            order.Refunds ??= [];
            order.Refunds.Add(refund);
            this._orderDataAccess.Update(order);
            return refund;
        }

    }
}
