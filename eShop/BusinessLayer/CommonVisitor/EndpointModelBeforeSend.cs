using Common;
using Common.Base;
using Common.Interface;

namespace BusinessLayer.CommonVisitor;

public class EndpointModelBeforeSend : IVisitorCaller<Address, AdminUser, Category, Customer, FinancialTransaction, HistoryLog, Order, Product, Review>
{
    public Address Visit(Address visitor) => RemoveId(visitor);

    public Order Visit(Order visitor) => RemoveId(visitor);

    public FinancialTransaction Visit(FinancialTransaction visitor) => RemoveId(visitor);

    public Product Visit(Product visitor) => RemoveId(visitor);

    public AdminUser Visit(AdminUser visitor) => RemoveId(visitor);

    public Category Visit(Category visitor) => RemoveId(visitor);

    public Customer Visit(Customer visitor) => RemoveId(visitor);

    public HistoryLog Visit(HistoryLog visitor) => RemoveId(visitor);

    public Review Visit(Review visitor) => RemoveId(visitor);

    private T RemoveId<T>(IElement<T>  el)
    {
        el.Id = null;
        return el;
    } 
}
