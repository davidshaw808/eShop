using Common;
using Common.Interface;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IOrderDataAccess : IUnitOfWorkCRUD<Order>, IAtomicCRUD<Order>
{
    Order? Get(Guid Key);
    Task<Order?> GetAsync(Guid Key);
    IEnumerable<Order> Get(Func<Order, bool> filter);
    IAsyncEnumerable<Order> GetAsync(Func<Order, bool> filter);
    IAsyncEnumerable<Order> GetOrdersAsync(Guid CustKey);
    bool Update(IEnumerable<Order> orders);
    bool Generate(FinancialTransaction paymentDetails);
    bool Update(PaymentDetails paymentDetails);
}
