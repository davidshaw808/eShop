using Common;
using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class OrderDataAccess : IOrderDataAccess
    {
        private readonly eShopBaseContext _db;

        public OrderDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool Delete(Common.Order t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            var o = this.Get((Guid)t.AltId);
            if(o == null)
            {
                return false;
            }
            o.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(Order t)
        {
            if (t.Id != null)
            {
                return false;
            }
            t.AltId = Guid.NewGuid();
            this._db.Orders.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(PaymentDetails pd)
        {
            if (pd.Id != null)
            {
                return false;
            }
            this._db.PaymentDetails.Add(pd);
            this._db.SaveChanges();
            return true;
        }

        public bool Update(PaymentDetails pd)
        {
            if (pd.Id == null)
            {
                this._db.PaymentDetails.Add(pd);
                return true;
            }
            this._db.PaymentDetails.Update(pd);
            this._db.SaveChanges();
            return true;
        }

        public Common.Order? Get(Guid altId)
        {
            return this._db.Orders.FirstOrDefault(o => o.AltId == altId);
        }

        public IEnumerable<Order> Get(Func<Order, bool> filter)
        {
            return this._db.Orders.Where(filter);
        }

        public bool Update(Order t)
        {
            this._db.Orders.Update(t);
            this._db.SaveChanges();
            return true;
        }
        public bool Update(IEnumerable<Order> t)
        {
            this._db.Orders.UpdateRange(t);
            this._db.SaveChanges();
            return true;
        }
    }
}
