using Common;
using Common.Enum;
using Common.Interface;

namespace BusinessLayer.Interface
{
    public interface IOrderService : IGenerateUpdateDelete<Order>
    {
        IEnumerable<Order>? GetAllAwaitingDelivery();
        bool AddOrderUpdate(Guid orderId, OrderUpdate update);
        bool AddRefund(Guid orderId, Refund refund);
        bool Generate(Customer customer, string paymentId, string? jsonPaymentResponse, decimal paidAmount, Currency currency, PaymentProvider paymentProvider, Address? a);
        Order? GetOrder(Guid orderId);

        bool AddOrderUpdate(Order order, OrderUpdate orderUpdate);
    }
}
