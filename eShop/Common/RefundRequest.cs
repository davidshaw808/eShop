using Common.Enum;
using Common.Interface;
using System.Text.Json.Serialization;

namespace Common
{
    public class FinancialTransaction : IElement<FinancialTransaction>, ISecureElement
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public Order Order { get; set; }
        public string Description { get; set; }
        public decimal? Amount { get; set; }
        public bool Active { get; set; }
        public DateTime DateGenerated { get; set; }
        public DateTime? DateApproved { get; set; }
        public DateTime? DatePaid { get; set; }
        public string? jsonPaymentProviderResponse {  get; set; }
        public PaymentProvider PaymentProvider { get; set; }

        public FinancialTransaction Visit(IVisitor<FinancialTransaction> visitor)
        {
            return visitor.Visit(this);
        }
    }

    public class RefundRequest : FinancialTransaction { }

    public class PaymentRequest : FinancialTransaction { }
}
