using BusinessLayer.Implementation.User;
using BusinessLayer.Interface.Admin;
using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using DataLayer.Interface;
using Newtonsoft.Json;
using System.Transactions;

namespace BusinessLayer.Implementation.Admin
{
    public class OrderServiceAdmin(IOrderDataAccess orderDataAccess,
        IFinancialTransaction refundDataAccess,
        IAddressOrderService addressOrderService,
        ICustomerOrderServiceAdmin customerOrderServiceAdmin,
        IProductOrderServiceAdmin productOrderService) : OrderService(orderDataAccess, customerOrderServiceAdmin), IOrderServiceAdmin
    {
        readonly IOrderDataAccess _orderDataAccess = orderDataAccess;
        readonly IFinancialTransaction _finTransactionDataAccess = refundDataAccess;
        readonly IAddressOrderService _addressOrderService = addressOrderService;
        readonly ICustomerOrderServiceAdmin _customerOrderServiceAdmin = customerOrderServiceAdmin;
        readonly IProductOrderServiceAdmin _productOrderService = productOrderService; 
   
        public IEnumerable<Order>? GetAllAwaitingDelivery()
        {
            return this._orderDataAccess.Get(o => !o.Delivered && o.Active);
        }

        public bool AddRefund(Guid orderId, RefundRequest refund)
        {
            var order = this._orderDataAccess.Get(orderId);
            if (order == null)
            {
                return false;
            }
            order.Refunds ??= new List<RefundRequest>();
            order.Refunds.Add(refund);
            return this._orderDataAccess.Update(order);
        }

        public Guid? Generate(Customer customer,
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
            using (var trScope = new TransactionScope())//MSDTC so this will only run on a Windows box
            {
                if (order.Address.AltId == null)
                {
                    this._addressOrderService.Generate(order.Address);
                }
                this._orderDataAccess.Generate(paymentDetails);
                this.AddOrderUpdate(order, OrderStatus.Paid, "Order generated");
                this._orderDataAccess.Generate(order);
                paymentDetails.Order = order;
                this._orderDataAccess.Update(paymentDetails);
                this._customerOrderServiceAdmin.AddOrder((Guid)customer.AltId, order);
                this._customerOrderServiceAdmin.Update(customer);
                var result = this._productOrderService.UpdateAfterSale(order);
                if (result.HasValue)
                { 
                    this.ProcessAfterSale(order, result.Value.DebitAmount, result.Value.CreditAmount, result.Value.Message);
                }
                this._customerOrderServiceAdmin.ClearBasket((Guid)customer.AltId);
                trScope.Complete();
            }
            return order.AltId;
        }

        

        public bool LogicalDelete(Order t)
        {
            if (t.AltId == null)
            {
                return false;
            }
            return _orderDataAccess.LogicalDelete(t);
        }

        public bool Update(Order order)
        {
            if (order.Id == null || order.AltId == null)
            {
                return false;
            }
            return _orderDataAccess.Update(order);
        }

        protected RefundRequest GenerateCreditRequest(Order order, decimal amount, string description)
        {
            var refund = new RefundRequest()
            {
                Order = order,
                Description = description,
                Amount = amount,
                DateGenerated = DateTime.UtcNow,
            };
            _finTransactionDataAccess.Generate(refund);
            return refund;
        }

        public bool UpdateRefund(Guid orderId,
            Guid refundId,
            string? jsonPaymentGatewayResponse,
            PaymentProvider provider)
        {
            var order = this._orderDataAccess.Get(orderId) ?? throw new ArgumentException("OrderId provided is invalid");
            var refund = order?.Refunds?.FirstOrDefault(r => r.AltId == refundId) ?? throw new ArgumentException("RefundId provided is invalid");
            refund.jsonPaymentProviderResponse = jsonPaymentGatewayResponse;
            refund.DatePaid = DateTime.UtcNow;
            refund.PaymentProvider = provider;
            return this._orderDataAccess.Update(order);
        }

        public bool BeforePayment(Customer customer)
        {
            throw new NotImplementedException();
        }

        protected PaymentRequest GenerateDebitRequest(Order order, decimal amount, string description)
        {
            var payment = new PaymentRequest()
            {
                Order = order,
                Description = description,
                Amount = amount,
                DateGenerated = DateTime.UtcNow,
            };
            _finTransactionDataAccess.Generate(payment);
            order.PaymentRequests ??= [];
            order.PaymentRequests.Add(payment);
            _orderDataAccess.Update(order);
            return payment;
        }

        protected bool AddOrderUpdate(Order order, OrderStatus orderStatus, string text)
        {
            if (order.AltId == null)
            {
                return false;
            }
            var ou = new OrderUpdate()
            {
                Order = order,
                Status = orderStatus,
                UpdateText = text
            };
            return AddOrderUpdate((Guid)order.AltId, ou);
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
                PaymentDetails = [paymentDetails],
                Active = true,
                Amount = customer.Basket?.Sum(p => p.Price) ?? 0
            };
            return (paymentDetails, order);
        }
        private void ProcessAfterSale(Order order, decimal debitAmount, decimal creditAmount, string message)
        {
            if (creditAmount > 0)
            {
                GenerateCreditRequest(order, creditAmount, message);
            }
            if (debitAmount > 0)
            {
                GenerateDebitRequest(order, debitAmount, message);
            }
        }

    }
}
