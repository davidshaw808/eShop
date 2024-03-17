using Common.Enum;
using Common.Interface;
using System.Text.Json.Serialization;

namespace Common
{
    public record PaymentDetails
    {
        [JsonIgnore]
        public int? Id { get; set; }
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
        [JsonIgnore]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdateText { get; set; }
        public OrderStatus Status { get; set; }

        public Order Order{ get; set; }
    }

    public class Order : IElement<Order>, ISecureElement
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } 
        public bool Active { get; set; }
        public Customer Customer { get; set; }
        public Address Address { get; set; }
        public IList<OrderUpdate>? Updates { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IList<RefundRequest>? Refunds { get; set; }
        public IList<PaymentRequest>? PaymentRequests { get; set; }
        public IList<PaymentDetails> PaymentDetails { get; set; }

        public bool Delivered { get; set; }
        public bool Cancelled { get; set; }

        public Order Visit(IVisitor<Order> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
