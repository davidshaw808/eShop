using Common.Enum;
using Common.Interface;

namespace Common
{
    public class Refund : IElement<Refund>, ISecureElement
    {
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public Order Order { get; set; }
        public string DescriptionOfRefund { get; set; }
        public decimal? Amount { get; set; }
        public bool Active { get; set; }
        public DateTime DateGenerated { get; set; }
        public DateTime? DateApproved { get; set; }
        public string? jsonPaymentProviderResponse {  get; set; }
        public PaymentProvider PaymentProvider { get; set; }

        public S Visit<S>(Func<Refund, S> visitor)
        {
            return visitor(this);
        }
    }
}
