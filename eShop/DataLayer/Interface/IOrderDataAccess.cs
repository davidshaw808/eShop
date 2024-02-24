using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IOrderDataAccess : IGenerateUpdateDelete<Order>
    {
        public Order? Get(Guid altId);
        public IEnumerable<Order> Get(Func<Order, bool> filter);
        public bool Update(IEnumerable<Order> orders);

        public bool Generate(PaymentDetails paymentDetails);
        public bool Update(PaymentDetails paymentDetails);
    }
}
