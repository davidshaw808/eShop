using DataLayer.Databases.Base;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class CustomerDataAccess : ICustomerDataAccess
    {
        private readonly eShopBaseContext _db;

        public CustomerDataAccess(eShopBaseContext db)
        {
            this._db = db;
        }

        public bool Delete(Common.Customer t)
        {
            if(t.AltId == null)
            {
                return false;
            }
            var cus = this.Get((Guid)t.AltId);
            if(cus == null) { 
                return false;            
            }  
            cus.Active = false;
            this._db.SaveChanges();
            return true;
        }

        public bool Generate(Common.Customer t)
        {
            if (t.Id != null)
            {
                return false;
            }
            t.AltId = Guid.NewGuid();
            this._db.Customers.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public Common.Customer? Get(Guid altId)
        {
            return _db.Customers.FirstOrDefault(c => c.AltId == altId);
        }

        public Common.Customer? Get(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Common.Customer> Get(Func<Common.Customer, bool> filter)
        {
            return this._db.Customers.Where(filter);
        }

        public bool PermanentlyRemoveAllCustomerData(Guid altId)
        {
            this._db.Customers
                .Where(c => c.AltId == altId)
                .ExecuteDelete();
            return true;
        }

        public bool Update(Common.Customer t)
        {
            this._db.Customers.Update(t);
            this._db.SaveChanges();
            return true;
        }
    }
}
