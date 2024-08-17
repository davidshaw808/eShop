using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace DataLayer.Implementation
{
    public class FinancialTransactionDataAccess: IFinancialTransactionDataAccess
    {
        private readonly eShopBaseContext _db;

        public FinancialTransactionDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool LogicalDelete(RefundRequest t)
        {
            if(t.AltId == null)
            {
                return false;
            }

            var r = this._db.RefundRequests.FirstOrDefault(r => r.AltId == t.AltId);
            if (r == null)
            {
                return false;
            }
            r.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(RefundRequest t)
        {
            if(t.Id != null) 
            {
                return false;
            }
            Populate(t);
            this._db.RefundRequests.Add(t);
            return this._db.SaveChanges() > 0;
        }

        public IEnumerable<RefundRequest> GetAll(Func<RefundRequest, bool> condition)
        {
           return this._db.RefundRequests.Where(condition);
        }

        public bool Update(RefundRequest t)
        {
            this._db.RefundRequests.Update(t);
            return this._db.SaveChanges() > 0;
        }

        public bool LogicalDelete(PaymentRequest t)
        {
            if (t.AltId == null)
            {
                return false;
            }
            var r = this._db.PaymentRequests.FirstOrDefault(r => r.AltId == t.AltId);
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
            this._db.PaymentRequests.Add(t);
            return this._db.SaveChanges() > 0;
        }

        public RefundRequest? Get(Guid id)
        {
            return this._db.RefundRequests.FirstOrDefault(r => r.AltId == id);
        }

        public IEnumerable<PaymentRequest> GetAll(Func<PaymentRequest, bool> condition)
        {
            return this._db.PaymentRequests.Where(condition);
        }

        public bool Update(PaymentRequest t)
        {
            this._db.PaymentRequests.Update(t);
            return this._db.SaveChanges() > 0;
        }

        public IAsyncEnumerable<PaymentRequest> GetAllAsync(Expression<Func<PaymentRequest, bool>> condition)
        {
            return this._db.PaymentRequests.Where(condition).AsQueryable().AsAsyncEnumerable();
        }

        public IAsyncEnumerable<RefundRequest?> GetAllAsync(Expression<Func<RefundRequest, bool>> condition)
        {
            return this._db.RefundRequests.Where(condition).AsAsyncEnumerable();
        }

        public Task<int> GenerateAsync(RefundRequest t)
        {
            if (t.Id != null)
            {
                return false;
            }
            Populate(t);
            this._db.RefundRequests.Add(t);
            return this._db.SaveChangesAsync();
        }

        public Task<bool> UpdateASync(RefundRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogicalDeleteAsync(RefundRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GenerateAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogicalDeleteAsync(PaymentRequest t)
        {
            throw new NotImplementedException();
        }

        private void Populate(FinancialTransaction t)
        {
            t.AltId = Guid.NewGuid();
            t.DateGenerated = DateTime.UtcNow;
        }
    }
}
