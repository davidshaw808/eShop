using BusinessLayer.Interface;
using Common;
using Common.Enum;
using DataLayer.Implementation;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class OrderService : IOrderService
    {
        readonly IOrderDataAccess _orderDataAccess;
        readonly IRefundDataAccess _refundDataAccess;
        readonly ICustomerService _customerService;
        readonly IAddressService _addressService;
        readonly IProductService _productService;

        public OrderService(IOrderDataAccess orderDataAccess,
            IRefundDataAccess refundDataAccess,
            ICustomerService customerService,
            IAddressService addressService,
            IProductService productService) 
        { 
            this._orderDataAccess = orderDataAccess;
            this._refundDataAccess = refundDataAccess;
            this._customerService = customerService;
            this._addressService = addressService;
            this._productService = productService;
        }

        public bool AddOrderUpdate(Guid orderId, OrderUpdate update)
        {
            var order = this._orderDataAccess.Get(orderId);
            if(order == null || order.Delivered)
            {
                return false;
            }
            update.Order = order;
            if(update.Status == OrderStatus.Delivered)
            {
                order.Delivered = true;
            }
            this._orderDataAccess.Update(order);
            return true;
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
            this._orderDataAccess.Update(order);
            return true;
        }

        public bool Delete(Order t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            return this._orderDataAccess.Delete(t);
        }

        public bool Generate(Order t)
        {
            if (t.AltId != null || t.PaymentDetails == null)
            {
                return false;
            }
            
            if(t.Customer.AltId == null)
            {
                this._customerService.Generate(t.Customer);
                this._customerService.AddOrder((Guid)t.Customer.AltId, t);
            }
            if(t.Address.AltId == null)
            {
                this._addressService.Generate(t.Address);
            }
            if(t.PaymentDetails.Id == null)
            {
                this._orderDataAccess.Generate(t.PaymentDetails);
            }
            t.Active = true;
            this._orderDataAccess.Generate(t);
            var result = this._productService.UpdateAfterSale(t);
            if (result != null)
            {
                //log here
                GenerateRefund(t, result.Value.RefundAmount, result.Value.Message);
            }
            return true;
        }

        public bool Generate(Customer c,
            string paymentId,
            string jsonPaymentResponse,
            decimal paidAmount,
            Currency currency,
            PaymentProvider paymentProvider,
            Address? a)
        {
            if (c.AltId == null || (a == null && c.Address == null))
            {
                throw new InvalidDataException("Cannot complete order, it must contain an address.");
            }
            
            var pd = new PaymentDetails()
            {
                PaymentProviderId = paymentId,
                JsonPaymentProviderResponse = jsonPaymentResponse,
                Currency = currency,
                Amount = paidAmount,
                PaymentProvider = paymentProvider,
                Created = DateTime.UtcNow
            };
            this._orderDataAccess.Generate(pd);
            var o = new Order()
            {
                Customer = c,
                Address = a ?? c.Address,
                Products = c.Basket,
                PaymentDetails = pd,
                Active = true,
                Amount = c.Basket?.Sum(p => p.Price) ?? 0
            };
            if (o.Address.AltId == null)
            {
                this._addressService.Generate(o.Address);
            }
            this._orderDataAccess.Generate(o);
            pd.Order = o;
            this._orderDataAccess.Update(pd);
            this._customerService.AddOrder((Guid)c.AltId, o);
            this._customerService.Update(c);
            var result = this._productService.UpdateAfterSale(o);
            if (result != null)
            {
                //log here
                GenerateRefund(o, result.Value.RefundAmount, result.Value.Message);
            }
            return true;
        }

        public IEnumerable<Order>? GetAllAwaitingDelivery()
        {
            return this._orderDataAccess.Get(o => !o.Delivered && o.Active);
        }

        public bool Update(Order t)
        {
            if (t.Id == null || t.AltId == null)
            {
                return false;
            }
            return this._orderDataAccess.Update(t);
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
    }
}
