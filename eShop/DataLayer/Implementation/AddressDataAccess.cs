using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class AddressDataAccess(eShopBaseContext db) : IAddressDataAccess
    {
        private readonly eShopBaseContext _db = db;

        public Task<int> GenerateAsync(Address t)
        {
            if(!this.GenerateWithChecks(t))
            {
                return Task.FromResult(0);
            }
            this._db.Addresses.Add(t);
            return this._db.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(Address t)
        {
            this._db.Addresses.Update(t);
            return this._db.SaveChangesAsync();
        }

        public Task<Address?> GetAsync(Guid altId, bool active) => this._db.Addresses.FirstOrDefaultAsync(a => a.AltId.Equals(altId) && a.Active.Equals(active));

        public async Task<int> LogicalDeleteAsync(Address t, bool commit) => (this.SetDelete(t) && commit) ? await this._db.SaveChangesAsync() : 0;

        public int Generate(Address t, bool commit)
        {
            if (!this.GenerateWithChecks(t))
            {
                return 0;
            }
            this._db.Addresses.Add(t);
            if(commit)
            {
                 return this._db.SaveChanges();
            }
            return 0;
        }

        public int Update(Address t, bool commit)
        {
            this._db.Addresses.Update(t);
            if (commit)
            {
                return this._db.SaveChanges();
            }
            return 0;
        }

        public int LogicalDelete(Address t, bool commit) =>  (!this.SetDelete(t) || !commit) ? 0 : this._db.SaveChanges();

        public IAsyncEnumerable<Address> GetAllAsync(Guid custId, bool active)
        {
            return this._db.Customers
                .Where(c  => c.AltId.Equals(custId) && c.Active.Equals(active) && c.Address != null)
                .Select(c => c.Address)
                .AsAsyncEnumerable();
        }

        private bool GenerateWithChecks(Address address)
        {
            if (address.Id != null)
            {
                return false;
            }
            address.AltId = Guid.NewGuid();//set guid only do not set active status as it is feasible to generate a deactivated address
            return true;
        }
        
        public Task<int> LogicalDeleteAsync(Address t) => !this.SetDelete(t) ? Task.FromResult(0) : this._db.SaveChangesAsync();

        private bool SetDelete(Address address)
        {
            if (address == null)
            {
                return false;
            }
            this._db.Track(address);//we know the enitity exists (as you cannot delete something if it's not present) so at the very lease ensure the deactivation is tracked - no need to send back the other changes if already made and not already attached
            address.Active = false;
            return true;
        }

        
    }
}
