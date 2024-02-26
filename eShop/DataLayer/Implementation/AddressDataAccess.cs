using DataLayer.Databases.Base;
using DataLayer.Interface;

namespace DataLayer.Implementation
{
    public class AddressDataAccess : IAddressDataAccess
    {
        private readonly CrispHabitatBaseContext _db;

        public AddressDataAccess(CrispHabitatBaseContext db) {
            this._db = db;
        }

        public bool Delete(Common.Address t)
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
            /*if(t.Id == null || t.AltId == null)
            {
                return false;
            }
            var a = this.Get((Guid)t.AltId);
            if (a == null)
            {
                return false;
            }
            this._db.Addresses.Update(a);
            a.Active = t.Active;
            a.AddressLines = t.AddressLines;
            if (t.CityTown != null) { a.CityTown = t.CityTown; };
            a.HouseNameNumber = t.HouseNameNumber;
            if (t.PostalCode != null) { a.PostalCode = t.PostalCode; };
            a.Region = t.Region;*/
            this._db.Addresses.Update(t);
            this._db.SaveChanges();
            return true;
        }
    }
}
