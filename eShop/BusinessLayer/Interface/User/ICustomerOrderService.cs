using Common;

namespace BusinessLayer.Interface.User
{
    public interface ICustomerOrderService
    {
        public (bool CanProcess, string Message) CanProcessBasket(Guid altCustId);
        public bool ClearBasket(Guid altCustId);
        IEnumerable<Product> PrepareBasketPayment(Guid altCustId);
        bool Update(Customer customer);
        bool Generate(Customer customer);

    }
}
