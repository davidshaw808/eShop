using Common;
using Common.Interface;
using System.Linq.Expressions;

namespace DataLayer.Interface
{
    public interface IFinancialTransactionDataAccess
    {
        RefundRequest? Get(Guid id);
        IEnumerable<PaymentRequest?> GetAll(Func<PaymentRequest, bool> condition);
        IEnumerable<RefundRequest?> GetAll(Func<RefundRequest, bool> condition);

        IAsyncEnumerable<PaymentRequest?> GetAllAsync(Expression<Func<PaymentRequest, bool>> condition);
        IAsyncEnumerable<RefundRequest?> GetAllAsync(Expression<Func<RefundRequest, bool>> condition);

        bool Generate(RefundRequest t);
        bool Update(RefundRequest t);
        bool LogicalDelete(RefundRequest t);
        Task<bool> GenerateAsync(RefundRequest t);
        Task<bool> UpdateASync(RefundRequest t);
        Task<bool> LogicalDeleteAsync(RefundRequest t);

        bool Generate(PaymentRequest t);
        bool Update(PaymentRequest t);
        bool LogicalDelete(PaymentRequest t);
        Task<bool> GenerateAsync(PaymentRequest t);
        Task<bool> UpdateAsync(PaymentRequest t);
        Task<bool> LogicalDeleteAsync(PaymentRequest t);
    }
}
