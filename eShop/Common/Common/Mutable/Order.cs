using Common.Enum;
using Common.Interface;
using Common.Models.Immutable;

namespace Common.Models.Mutable;

public sealed class Order : IElement<Order>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public bool Active { get; set; }
    public Customer Customer { get; set; }
    public string Address { get; set; }
    public ICollection<OrderUpdate>? Updates { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<FinancialTransaction>? Payments { get; set; }
    public ICollection<PaymentRequest>? PaymentRequests { get; set; }
    public DateTime DateApproved { get; set; }
    public Shipping Shipping { get; set; }
    public Vat VatApplied { get; set; }

    public U Visit<U>(IVisitor<Order, U> visitor)
    {
        return visitor.Visit(this);
    }
}

public class OrderUpdate
{
    public DateTime CreatedDate { get; set; }
    public string UpdateText { get; set; }
    public OrderStatus Status { get; set; }

    public Order Order { get; set; }
}
