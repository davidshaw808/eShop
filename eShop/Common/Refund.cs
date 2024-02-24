using Common.Interface;

namespace Common
{
    public class Refund : IElement<Refund>, ISecureElement
    {
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public IList<Order> Orders { get; set; }
        public string DescriptionOfRefund { get; set; }
        public decimal? Amount { get; set; }
        public bool Active { get; set; }

        public S Visit<S>(Func<Refund, S> visitor)
        {
            return visitor(this);
        }
    }
}
