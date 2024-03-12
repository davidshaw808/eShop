using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class RefundDataAccess: IRefundDataAccess
    {
        private readonly eShopBaseContext _db;

        public RefundDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool Delete(Refund t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            var r = this.Get((Guid)t.AltId);
            if(r == null)
            {
                return false;
            }
            r.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(Refund t)
        {
            if(t.Id != null) 
            {
                return false;
            }
            t.AltId = Guid.NewGuid();
            t.DateGenerated = DateTime.UtcNow;
            this._db.Refunds.Add(t);
            return this._db.SaveChanges() > 0;
        }

        public Common.Refund? Get(Guid id)
        {
            return this._db.Refunds.FirstOrDefault(r => r.AltId == id);
        }

        public IEnumerable<Refund> GetAllRequiringApproval()
        {
           return this._db.Refunds.Where(r => r.DateApproved == null);
        }

        public bool Update(Refund t)
        {
            this._db.Refunds.Update(t);
            return this._db.SaveChanges() > 0;
        }
    }
}
