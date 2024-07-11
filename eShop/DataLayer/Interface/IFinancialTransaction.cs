using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IFinancialTransaction
    {
        RefundRequest? Get(Guid id);
        IEnumerable<PaymentRequest?> GetAll(Func<PaymentRequest, bool> condition);
        IEnumerable<RefundRequest?> GetAll(Func<RefundRequest, bool> condition);

        Task<IEnumerable<PaymentRequest?>> GetAllAsync(Func<PaymentRequest, bool> condition);
        Task<IEnumerable<RefundRequest?>> GetAllAsync(Func<RefundRequest, bool> condition);

        bool Generate(RefundRequest t);
        bool Update(RefundRequest t);
        bool LogicalDelete(RefundRequest t);

        bool Generate(PaymentRequest t);
        bool Update(PaymentRequest t);
        bool LogicalDelete(PaymentRequest t);
    }
}
