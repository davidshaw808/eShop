using Common.Models.Immutable;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataLayer.Implementation
{
    public class FinancialTransactionDataAccess: IFinancialTransactionDataAccess
    {
        private readonly eShopBaseContext _db;
        Func<eShopBaseContext, Guid, Task<FinancialTransaction?>> _getFinancialTransactionNoTrack;
        Func<eShopBaseContext, Guid, Task<FinancialTransaction?>> _getFinancialTransactionTracking;

        public FinancialTransactionDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            _db = unitOfWork.GetContext();
            _getFinancialTransactionNoTrack = GetCompiledFinancialTransactionNoTrack();
            _getFinancialTransactionTracking = GetCompiledFinancialTransactionTracking();
        }

        public async Task<IEnumerable<FinancialTransaction>> GetAllAsync(Func<FinancialTransaction, bool> condition)
        {
           return _db.FinancialTransactions.Where(condition);
        }

        public async Task<FinancialTransaction?> GetAtomicAsync(Guid key) => await _getFinancialTransactionNoTrack(_db, key);

        public IAsyncEnumerable<FinancialTransaction?> GetAllAsync(Expression<Func<FinancialTransaction, bool>> condition)
        {
            return _db.FinancialTransactions.Where(condition).AsAsyncEnumerable();
        }

        public async Task<bool> GenerateAtomicAsync(FinancialTransaction financialTranasaction)
        {
            var task = _db.FinancialTransactions.SingleInsertAsync(financialTranasaction);
            await task;
            return task.IsCompletedSuccessfully;
        }

        public async Task<FinancialTransaction?> GetInUoW(Guid key) => await _getFinancialTransactionTracking(_db, key);

        public void GenerateInUoW(FinancialTransaction financialTranasaction)
        {
            var task = _db.FinancialTransactions.Add(financialTranasaction);
        }

        private Func<eShopBaseContext, Guid, Task<FinancialTransaction?>> GetCompiledFinancialTransactionNoTrack() => EF.CompileAsyncQuery(
            (eShopBaseContext db, Guid key) => db.FinancialTransactions
            .AsNoTrackingWithIdentityResolution()
            .Include(ft => ft.Order)
            .Include(ft => ft.PaymentRequest)
            .Include(ft => ft.Vat)
            .Where(ft => ft.Key.Equals(key))
            .FirstOrDefault()
        );

        private Func<eShopBaseContext, Guid, Task<FinancialTransaction?>> GetCompiledFinancialTransactionTracking() => EF.CompileAsyncQuery(
            (eShopBaseContext db, Guid key) => db.FinancialTransactions
            .Include(ft => ft.Order)
            .Include(ft => ft.PaymentRequest)
            .Include(ft => ft.Vat)
            .Where(ft => ft.Key.Equals(key))
            .FirstOrDefault()
        );
    }
}
