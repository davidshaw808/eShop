using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;
using Newtonsoft.Json;

namespace BusinessLayer.Implementation.Admin
{
    public class OrderServiceAdmin(IOrderDataAccess orderDataAccess,
        IRefundDataAccess refundDataAccess,
        IAddressOrderService addressOrderService,
        ICustomerOrderServiceAdmin customerOrderService,
        IProductOrderServiceAdmin productOrderService) : OrderService(orderDataAccess,
            refundDataAccess,
            addressOrderService,
            customerOrderService,
            productOrderService), IOrderServiceAdmin, IOrderService
    {
        readonly IOrderDataAccess _orderDataAccess = orderDataAccess;
        readonly IRefundDataAccess _refundDataAccess = refundDataAccess;
        readonly IAddressOrderService _addressOrderService = addressOrderService;
        readonly ICustomerOrderServiceAdmin _customerOrderService = customerOrderService;
        readonly IProductOrderServiceAdmin _productOrderService = productOrderService; 
   
        public IEnumerable<Order>? GetAllAwaitingDelivery()
        {
            return this._orderDataAccess.Get(o => !o.Delivered && o.Active);
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

        public bool Generate(Customer customer,
            string paymentId,
            string? jsonPaymentResponse,
            decimal paidAmount,
            Currency currency,
            PaymentProvider paymentProvider,
            Address? address)
        {
            var orderResult = BuildOrder(paymentId, jsonPaymentResponse, currency, paidAmount, paymentProvider, customer, address);
            var paymentDetails = orderResult.paymentDetails;
            var order = orderResult.order;
            if (order.Address.AltId == null)
            {
                this._addressOrderService.Generate(order.Address);
            }
            this._orderDataAccess.Generate(paymentDetails);
            base.AddOrderUpdate(order, OrderStatus.Paid, "Order generated");
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
        
        protected (PaymentDetails paymentDetails, Order order) BuildOrder(string paymentId,
            string? jsonPaymentResponse,
            Currency currency,
            decimal paidAmount,
            PaymentProvider paymentProvider,
            Customer customer,
            Address? address
            )
        {
            if (customer.AltId == null || (address == null && customer.Address == null))
            {
                throw new InvalidDataException("Cannot complete order, it must contain an address.");
            }
            var paymentDetails =  new PaymentDetails()
            {
                PaymentProviderId = paymentId,
                JsonPaymentProviderResponse = jsonPaymentResponse,
                Currency = currency,
                Amount = paidAmount,
                PaymentProvider = paymentProvider,
                Created = DateTime.UtcNow
            };
            var order = new Order()
            {
                Customer = customer,
                Address = address ?? customer.Address,
                Products = customer.Basket,
                PaymentDetails = paymentDetails,
                Active = true,
                Amount = customer.Basket?.Sum(p => p.Price) ?? 0
            };
            return (paymentDetails, order);
        }

        public IEnumerable<Refund> GetAllOutstandingRefunds()
        {
            return this._refundDataAccess.GetAllRequiringApproval();
        }

        public bool UpdateRefund(Guid orderId,
            Guid refundId,
            string? jsonPaymentGatewayResponse,
            PaymentProvider provider)
        {
            var order = this._orderDataAccess.Get(orderId) ?? throw new ArgumentException("OrderId provided is invalid");
            var refund = order?.Refunds?.FirstOrDefault(r => r.AltId == refundId) ?? throw new ArgumentException("RefundId provided is invalid");
            refund.jsonPaymentProviderResponse = jsonPaymentGatewayResponse;
            refund.DateApproved = DateTime.UtcNow;
            refund.PaymentProvider = provider;
            return this._orderDataAccess.Update(order);
        }
    }
}
