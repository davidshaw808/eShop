using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IFinancialTransaction
    {
        public RefundRequest? Get(Guid id);
        public IEnumerable<PaymentRequest> GetAll(Func<PaymentRequest, bool> condition);
        public IEnumerable<RefundRequest> GetAll(Func<RefundRequest, bool> condition);

        public bool Generate(RefundRequest t);
        public bool Update(RefundRequest t);
        public bool LogicalDelete(RefundRequest t);

        public bool Generate(PaymentRequest t);
        public bool Update(PaymentRequest t);
        public bool LogicalDelete(PaymentRequest t);
    }
}
