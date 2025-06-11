using Common.Interface;
using Common.Models.Immutable;
using Common.Models.Mutable;

namespace BusinessLayer.Visitor;

public class EndpointModelBeforeSend : IVisitorCaller<Address, AdminUser, Category, Customer, FinancialTransaction, HistoryLog, Order, Product, Review>
{
    public Address Visit(Address visitor) => RemoveId(visitor);

    public Order Visit(Order visitor) => new Order()
    {
        Id = 0,
        Active = visitor.Active,
        Address = visitor.Address,
        Amount = visitor.Amount,
        DateApproved = visitor.DateApproved,
        PaymentRequests = visitor.PaymentRequests,  
        Payments = visitor.Payments,
        Products = visitor.Products,
        Currency = visitor.Currency,
        Customer = visitor.Customer,
        Key = visitor.Key,
        Shipping = visitor.Shipping,
        Updates = visitor.Updates,
        VatApplied = visitor.VatApplied,
    };

    public FinancialTransaction Visit(FinancialTransaction visitor) => new FinancialTransaction() { 
        Active = visitor.Active,
        AmountNet = visitor.AmountNet,
        Credit = visitor.Credit,
        DatePaid = visitor.DatePaid,
        Description = visitor.Description,
        Order = visitor.Order,
        Key = visitor.Key,

        PaymentProvider = visitor.PaymentProvider,
        Id = 0,
        JsonPaymentProviderResponse = string.Empty
    };

    public Product Visit(Product visitor) => RemoveId(visitor);

    public AdminUser Visit(AdminUser visitor) => RemoveId(visitor);

    public Category Visit(Category visitor) => RemoveId(visitor);

    public Customer Visit(Customer visitor) => RemoveId(visitor);

    public HistoryLog Visit(HistoryLog visitor) => RemoveId(visitor);

    public Review Visit(Review visitor) => RemoveId(visitor);

    private static T RemoveId<T>(IElement<T>  el)
    {
        el.Id = null;
        return (T)el;
    }
}
