using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using Common.Interface;

namespace BusinessLayer.Interface.Admin
{
    public interface IOrderServiceAdmin : IOrderService
    {
        bool Update(Order t);
        bool LogicalDelete(Order t);
        IEnumerable<Order>? GetAllAwaitingDelivery();
        bool AddRefund(Guid orderId, RefundRequest refund);
        bool BeforePayment(Customer customer);
        bool UpdateRefund(Guid orderId, Guid refundId, string? jsonPaymentResponse, PaymentProvider provider);
        Guid? Generate(Customer customer, string paymentId, string? jsonPaymentResponse, decimal paidAmount, Currency currency, PaymentProvider paymentProvider, Address? a);
    }
}
