using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class OrderService : OrderCustomerService, IOrderServiceAdmin, IOrderService
    {
        readonly IOrderDataAccess _orderDataAccess;
        readonly IRefundDataAccess _refundDataAccess;
        readonly IAddressOrderService _addressOrderService;
        readonly ICustomerOrderServiceAdmin _customerOrderService;
        readonly IProductOrderServiceAdmin _productOrderService;

        public OrderService(IOrderDataAccess orderDataAccess,
            IRefundDataAccess refundDataAccess,
            IAddressOrderService addressOrderService,
            ICustomerOrderServiceAdmin customerOrderService,
            IProductOrderServiceAdmin productOrderService): base(orderDataAccess)
        { 
            this._orderDataAccess = orderDataAccess;
            this._refundDataAccess = refundDataAccess;
            this._customerOrderService = customerOrderService;
            this._addressOrderService = addressOrderService;
            this._productOrderService = productOrderService;
        }

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

        public bool AddRefund(Guid orderId, Refund refund)
        {
            var order = this._orderDataAccess.Get(orderId);
            if (order == null)
            {
                return false;
            }
            order.Refunds ??= new List<Refund>();
            order.Refunds.Add(refund);
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

        public bool Generate(Customer customer,
            string paymentId,
            string jsonPaymentResponse,
            decimal paidAmount,
            Currency currency,
            PaymentProvider paymentProvider,
            Address? address)
        {
            if (customer.AltId == null || (address == null && customer.Address == null))
            {
                throw new InvalidDataException("Cannot complete order, it must contain an address.");
            }
            
            var paymentDetails = GenerateOrder(paymentId, jsonPaymentResponse, currency, paidAmount, paymentProvider);
            this._orderDataAccess.Generate(paymentDetails);
            var order = new Order()
            {
                Customer = customer,
                Address = address ?? customer.Address,
                Products = customer.Basket,
                PaymentDetails = paymentDetails,
                Active = true,
                Amount = customer.Basket?.Sum(p => p.Price) ?? 0
            };
            if (order.Address.AltId == null)
            {
                this._addressOrderService.Generate(order.Address);
            }
            AddOrderUpdate(order, OrderStatus.Paid, "Order generated");
            this._orderDataAccess.Generate(order);
            paymentDetails.Order = order;
            this._orderDataAccess.Update(paymentDetails);
            this._customerOrderService.AddOrder((Guid)customer.AltId, order);
            this._customerOrderService.Update(customer);
            var result = this._productOrderService.UpdateAfterSale(order);
            if (result != null)
            {
                //log here
                GenerateRefund(order, result.Value.RefundAmount, result.Value.Message);
            }
            return true;
        }

        public IEnumerable<Order>? GetAllAwaitingDelivery()
        {
            return this._orderDataAccess.Get(o => !o.Delivered && o.Active);
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

        private Refund GenerateRefund(Order o, decimal amount, string description)
        {
            var refund = new Refund()
            {
                Orders = [o],
                DescriptionOfRefund = description,
                Amount = amount
            };
            this._refundDataAccess.Generate(refund);
            o.Refunds ??= [];
            o.Refunds.Add(refund);
            this._orderDataAccess.Update(o);
            return refund;
        }

        private PaymentDetails GenerateOrder(string paymentId, string jsonPaymentResponse, Currency currency, decimal paidAmount, PaymentProvider paymentProvider)
        {
            return new PaymentDetails()
            {
                PaymentProviderId = paymentId,
                JsonPaymentProviderResponse = jsonPaymentResponse,
                Currency = currency,
                Amount = paidAmount,
                PaymentProvider = paymentProvider,
                Created = DateTime.UtcNow
            };
        }

        private bool AddOrderUpdate(Order order, OrderStatus orderStatus, string text)
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

        
    }
}
