using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation
{
    public class AddressDataAccess : IAddressDataAccess
    {
        private readonly eShopBaseContext _db;
        private readonly Func<eShopBaseContext,Guid,bool,Task<Address?>> _addressGetter;
        private readonly Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> _customerAddressGetter;
        private static readonly Task<int> _taskResult = Task.FromResult(0);

        public AddressDataAccess(eShopBaseContext db)
        {
            this._db = db;
            this._addressGetter = EF.CompileAsyncQuery((eShopBaseContext db, Guid altId, bool active) => db.Addresses.FirstOrDefault(a => a.AltId.Equals(altId) && a.Active.Equals(active)));
            _customerAddressGetter = EF.CompileAsyncQuery(
                (eShopBaseContext db, Guid custId, bool active) => db.Addresses
                .Include(a => a.Customer)
                .Where(a => a.Customer.AltId.Equals(custId) && a.Active.Equals(active))
                .AsAsyncEnumerable());
        }

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

        public Task<Address?> GetAsync(Guid altId, bool active) => this._addressGetter(_db, altId, active);

        public Task<IAsyncEnumerable<Address>> GetAllAsync(Guid custId, bool active) => this._customerAddressGetter(this._db, custId, active);

        public Task<int> LogicalDeleteAsync(Address t, bool commit) => (this.SetDelete(t) && commit) ? this._db.SaveChangesAsync() : _taskResult;

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

        /*._db.Customers
                .Where(c  => c.AltId.Equals(custId) && c.Active.Equals(active) && c.Address != null)
                .Select(c => c.Address)
                .AsAsyncEnumerable();
        }*/

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
            this._db.Track(address);
            address.Active = false;
            return true;
        }
    }
}
