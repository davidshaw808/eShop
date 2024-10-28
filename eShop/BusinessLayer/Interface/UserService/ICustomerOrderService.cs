using Common;

namespace BusinessLayer.Interface.User;

public interface ICustomerOrderService
{
    public ValueTask<IAsyncEnumerable<Order>?> GetCustomerOrderHistoryAsync(Customer customer);
    public (bool CanProcess, string Message) CanProcessBasket(Customer customer);
    public ValueTask<bool> ClearBasket(Customer customer);
    IEnumerable<Product> PrepareBasketPayment(Customer customer);
    bool Update(Customer customer);
    ValueTask<int> Generate(Customer customer);
    bool AddOrder(Customer customer, Order order);
}
