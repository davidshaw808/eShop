using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class AddressDataAccess : IAddressDataAccess
    {
        private readonly eShopBaseContext _db;

        public AddressDataAccess(eShopBaseContext db) 
        {
            this._db = db;
        }

        public bool LogicalDelete(Common.Address t)
        {
            var add = this._db.Addresses.FirstOrDefault(a => a.Id.Equals(t.Id));
            if (add != null)
            {
                add.Active = false;
                this._db.SaveChanges();
                return true;
            }
            return false;
        }

        public bool Generate(Common.Address t)
        {
            if(t.Id != null)
            {
                return false;
            }
            t.AltId = Guid.NewGuid();
            this._db.Addresses.Add(t);
            this._db.SaveChanges();
            return true;
        }

        public Common.Address? Get(Guid altId)
        {
            return this._db.Addresses.FirstOrDefault(a => a.AltId.Equals(altId));
        }

        public bool Update(Common.Address t)
        {
            this._db.Addresses.Update(t);
            this._db.SaveChanges();
            return true;
        }
    }
}
