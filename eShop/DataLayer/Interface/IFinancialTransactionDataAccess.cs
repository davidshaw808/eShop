using Common;
using Common.Models.Immutable;
using DataLayer.Interface.General;
using System.Linq.Expressions;

namespace DataLayer.Interface;

public interface IFinancialTransactionDataAccess : IUnitOfWorkImmutable<FinancialTransaction>, IAtomicImmutable<FinancialTransaction>
{
    Task<IEnumerable<FinancialTransaction?>> GetAllAsync(Func<FinancialTransaction, bool> condition);
    IAsyncEnumerable<FinancialTransaction?> GetAllAsync(Expression<Func<FinancialTransaction, bool>> condition);
}
