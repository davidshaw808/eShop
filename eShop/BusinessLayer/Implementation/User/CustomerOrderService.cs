using BusinessLayer.ClassHelpers.Extensions;
using BusinessLayer.Interface.User;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User
{
    public class CustomerOrderService(ICustomerDataAccess customerDataAccess): ICustomerOrderService
    {
        private readonly ICustomerDataAccess _customerDataAccess = customerDataAccess;

        public (bool CanProcess, string Message) CanProcessBasket(Guid altCustId)
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

        public bool ClearBasket(Guid altCustId)
        {
            var customer = this._customerDataAccess.Get(altCustId);
            if(customer == null)
            {
                return false;
            }
            customer.Basket?.Clear();
            customer.LockBasket = false;
            return this._customerDataAccess.Update(customer);
        }

        public bool Generate(Customer customer)
        {
            if(customer.AltId != null)
            {
                return false;
            }
            customer.AltId = Guid.NewGuid();
            return _customerDataAccess.Generate(customer);
        }

        public IEnumerable<Product> PrepareBasketPayment(Guid altCustId)
        {
            var customer = this._customerDataAccess.Get(altCustId);
            customer.LockBasket = true;
            this._customerDataAccess.Update(customer);
            return customer.Basket;
        }

        public bool Update(Customer customer)
        {
            return _customerDataAccess.Update(customer);
        }
    }
}
