using BusinessLayer.Interface.User;
using Common;
using Common.Enum;
using Common.Interface;

namespace BusinessLayer.Interface.Admin
{
    public interface IOrderServiceAdmin : IGenerateUpdateDelete<Order>, IOrderService
    {
        IEnumerable<Order>? GetAllAwaitingDelivery();
        bool AddRefund(Guid orderId, Refund refund);
        bool Generate(Customer customer, string paymentId, string? jsonPaymentResponse, decimal paidAmount, Currency currency, PaymentProvider paymentProvider, Address? a);
    }
}
