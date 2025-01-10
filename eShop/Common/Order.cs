using Common.Enum;
using Common.Interface;

namespace Common;

public class Order : IElement<Order>
{ 
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; } 
    public bool Active { get; set; }
    public Customer Customer { get; set; }
    public Address Address { get; set; }
    public ICollection<OrderUpdate>? Updates { get; set; }
    public ICollection<Product> Products { get; set; }
    public ICollection<FinancialTransaction>? Refunds { get; set; }
    public ICollection<FinancialTransaction>? PaymentRequests { get; set; }
    public ICollection<PaymentDetails> PaymentDetails { get; set; }

    public bool Delivered { get; set; }
    public bool Cancelled { get; set; }

    public U Visit<U>(IVisitor<Order,U> visitor)
    {
        return visitor.Visit(this);
    }
}

public record PaymentDetails
{
    public Order? Order { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PaymentProvider PaymentProvider { get; set; }

    public string PaymentProviderId { get; set; }
    public string? JsonPaymentProviderResponse { get; set; }
    public DateTime Created { get; set; }
}

public record OrderUpdate
{
    public DateTime CreatedDate { get; set; }
    public string UpdateText { get; set; }
    public OrderStatus Status { get; set; }

    public Order Order{ get; set; }
}
