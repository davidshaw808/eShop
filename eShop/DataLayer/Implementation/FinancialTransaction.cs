using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class FinancialTransaction: IFinancialTransaction
    {
        private readonly eShopBaseContext _db;

        public FinancialTransaction(eShopBaseContext db)
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
            t.AltId = Guid.NewGuid();
            t.DateGenerated = DateTime.UtcNow;
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
            t.AltId = Guid.NewGuid();
            t.DateGenerated = DateTime.UtcNow;
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
    }
}
