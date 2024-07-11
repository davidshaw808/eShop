using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IOrderDataAccess : IGenerateUpdateDelete<Order>
    {
        Order? Get(Guid altId);
        Task<Order?> GetAsync(Guid altId);
        IEnumerable<Order> Get(Func<Order, bool> filter);
        bool Update(IEnumerable<Order> orders);

        bool Generate(PaymentDetails paymentDetails);
        bool Update(PaymentDetails paymentDetails);
    }
}
