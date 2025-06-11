using BusinessLayer.ClassHelpers.Extensions;
using BusinessLayer.Interface.User;
using Common.Models.Immutable;
using Common.Models.Mutable;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User;

public class CustomerOrderService(ICustomerDataAccess customerDataAccess): ICustomerOrderService
{
    private readonly ICustomerDataAccess _customerDataAccess = customerDataAccess;

    public (bool CanProcess, string Message) CanProcessBasket(Customer customer)
    {
        var products = this._customerDataAccess.Get(altCustId)?.Basket;
        if (products == null)
        {
            return (false, "No products in basket");
        }
        if(products.Any(p => p.Id == null || p.NumberInStock == null))
        {
            var error = products
                .Where(p => p.Id == null || p.NumberInStock == null)
                .Select(p => p.Name);
            var errorMessage = string.Join(", ", error);
            return (false, $"Product(s) '{errorMessage}' either have invalid Id's or stock numbers");
        }
        var prodList = products.GroupProducts();
        foreach (var p in prodList)
        {
            var instock = products.FirstOrDefault(prod =>  prod.Id == p.Id).NumberInStock;
            if (p.Quantity > instock)
            {
                return (false, $"'{p.Name}' has {p.Quantity} in basket but only {instock} in stock");
            }
        }
        return (true, "");
    }

    public Task<bool> ClearBasket(Customer customer)
    {
        customer.BasketItems?.Clear();
        customer.LockBasket = false;
        using ()
            return this._customerDataAccess.UpdateAsync(customer);
    }

    public Task<int> Generate(Customer customer)
    {
        customer.Key = Guid.NewGuid();
        return _customerDataAccess.GenerateAsync(customer);
    }

    public async Task<IEnumerable<Product>> PrepareBasketPayment(Customer customer)
    {
        if(customer.Key == null)
        {
            throw new ArgumentException("Invalid custoemr obect no Key found");
        }
        customer.LockBasket = true;
        await this._customerDataAccess.upda(customer);
        var currentCustomer = await this._customerDataAccess.GetAtomicAsync((Guid)customer.Key);
        currentCustomer.BasketItems = new List<Product>();
        return currentCustomer?.BasketItems ?? Enumerable.Empty<Product>();
    }

    public bool Update(Customer customer)
    {
        return _customerDataAccess.Update(customer);
    }

    public bool AddOrder(Guid id, Order order)
    {
        var customer = _customerDataAccess.Get(id);
        if (customer == null)
        {
            return false;
        }
        if (customer.OrderHistory == null)
        {
            customer.OrderHistory = new List<Order>();
        }
        customer.OrderHistory.Add(order);
        return true;
    }

    public bool AddItemToBasket(Guid customerKey, Product product)
    {
        if (product.Key == null)
        {
            return false;
        }
        var customer = _customerDataAccess.Get(customerKey);
        if (customer == null && !customer.LockBasket)
        {
            return false;
        }
        customer.Basket ??= new List<Product>();
        customer.Basket.Add(product);
        return true;
    }

    public Task<IAsyncEnumerable<Order>?> GetCustomerOrderHistoryAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Product> PrepareBasketPayment(Customer customer)
    {
        throw new NotImplementedException();
    }

    public bool AddOrder(Customer customer, Order order)
    {
        throw new NotImplementedException();
    }
}
