using Common.Models.Immutable;
using Common.Models.Mutable;

namespace BusinessLayer.Interface.User;

public interface ICustomerOrderService
{
    public Task<IAsyncEnumerable<Order>?> GetCustomerOrderHistoryAsync(Customer customer);
    public (bool CanProcess, string Message) CanProcessBasket(Customer customer);
    public Task<bool> ClearBasket(Customer customer);
    IEnumerable<Product> PrepareBasketPayment(Customer customer);
    bool Update(Customer customer);
    Task<int> Generate(Customer customer);
    bool AddOrder(Customer customer, Order order);
}
