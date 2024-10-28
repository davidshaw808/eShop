using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace DataLayer.Implementation
{
    public class FinancialTransactionDataAccess: IFinancialTransactionDataAccess
    {
        private readonly eShopBaseContext _db;

        public FinancialTransactionDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
        {
            this._db = unitOfWork.GetContext();
        }

        public bool LogicalDelete(FinancialTransaction t)
        {
            if(!t.Validate())
            {
                return false;
            }

            var r = this._db.FinancialTransactions.FirstOrDefault(r => r.Key == t.Key);
            if (r == null)
            {
                return false;
            }
            r.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(FinancialTransaction t)
        {
            if(t.Id != null) 
            {
                return false;
            }
            Populate(t);
            this._db.FinancialTransactions.Add(t);
            return this._db.SaveChanges() > 0;
        }

        public IEnumerable<FinancialTransaction> GetAll(Func<FinancialTransaction, bool> condition)
        {
           return this._db.FinancialTransactions.Where(condition);
        }

        public bool Update(FinancialTransaction t)
        {
            this._db.FinancialTransactions.Update(t);
            return this._db.SaveChanges() > 0;
        }

        public bool LogicalDelete(PaymentRequest t)
        {
            if (!t.Validate())
            {
                return false;
            }
            var r = this._db.FinancialTransactions.FirstOrDefault(r => r.Key == t.Key);
            if (r == null)
            {
                return false;
            }
            r.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(PaymentRequest t)
        {
            if (t.Id != null)
            {
                return false;
            }
            Populate(t);
            this._db.FinancialTransactions.Add(t);
            return this._db.SaveChanges() > 0;
        }

        public FinancialTransaction? Get(Guid id)
        {
            return this._db.FinancialTransactions.FirstOrDefault(r => r.Key == id);
        }

        public IEnumerable<PaymentRequest> GetAll(Func<PaymentRequest, bool> condition)
        {
            return this._db.FinancialTransactions.Where(condition);
        }

        public bool Update(PaymentRequest t)
        {
            this._db.FinancialTransactions.Update(t);
            return this._db.SaveChanges() > 0;
        }

        public IAsyncEnumerable<PaymentRequest> GetAllAsync(Expression<Func<PaymentRequest, bool>> condition)
        {
            return this._db.FinancialTransactions.Where(condition).AsAsyncEnumerable();
        }

        public IAsyncEnumerable<FinancialTransaction?> GetAllAsync(Expression<Func<FinancialTransaction, bool>> condition)
        {
            return this._db.FinancialTransactions.Where(condition).AsAsyncEnumerable();
        }

        public Task<int> GenerateAsync(FinancialTransaction t)
        {
            if (t.Id != null)
            {
                return Task.FromResult(0);
            }
            Populate(t);
            this._db.FinancialTransactions.Add(t);
            return this._db.SaveChangesAsync();
        }

        public Task<int> UpdateASync(FinancialTransaction t)
        {
            this._db.FinancialTransactions.Update(t);
            return this._db.SaveChangesAsync();
        }

        public Task<int> LogicalDeleteAsync(FinancialTransaction t)
        {
            
        }

        public Task<int> GenerateAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<int> LogicalDeleteAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        private void Populate(FinancialTransaction t)
        {
            t.Key = Guid.NewGuid();
            t.DateGenerated = DateTime.UtcNow;
        }

        private Func<eShopBaseContext, Guid, int?> GetCompiledFinancialTransactionId() => EF.CompileQuery(
        (eShopBaseContext db, Guid Key) => db.FinancialTransactions
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );
    }
}
