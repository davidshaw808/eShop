using Common;
using System.Linq.Expressions;

namespace DataLayer.Interface;

public interface IFinancialTransactionDataAccess
{
    FinancialTransaction? Get(Guid id);
    IEnumerable<PaymentRequest?> GetAll(Func<PaymentRequest, bool> condition);
    IEnumerable<FinancialTransaction?> GetAll(Func<FinancialTransaction, bool> condition);

    IAsyncEnumerable<PaymentRequest?> GetAllAsync(Expression<Func<PaymentRequest, bool>> condition);
    IAsyncEnumerable<FinancialTransaction?> GetAllAsync(Expression<Func<FinancialTransaction, bool>> condition);

    bool Generate(FinancialTransaction t);
    bool Update(FinancialTransaction t);
    bool LogicalDelete(FinancialTransaction t);
    Task<int> GenerateAsync(FinancialTransaction t);
    Task<int> UpdateASync(FinancialTransaction t);
    Task<int> LogicalDeleteAsync(FinancialTransaction t);

    bool Generate(PaymentRequest t);
    bool Update(PaymentRequest t);
    bool LogicalDelete(PaymentRequest t);
    Task<int> GenerateAsync(PaymentRequest t);
    Task<int> UpdateAsync(PaymentRequest t);
    Task<int> LogicalDeleteAsync(PaymentRequest t);
}
