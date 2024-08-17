using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
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

        public int LogicalDelete(Customer t, bool commit)
        {
            if(t.AltId == null)
            {
                return 0;
            }
            this._db.Track(t);
            t.Active = false;
            return commit ? this._db.SaveChanges() : 0;
        }

        public int Generate(Customer t, bool commit)
        {
            if (t.Id != null)
            {
                return 0;
            }
            t.AltId = Guid.NewGuid();
            this._db.Customers.Add(t);
            return commit ? this._db.SaveChanges() : 0;
        }

        public Customer? Get(Guid altId) => _db.Customers.FirstOrDefault(c => c.AltId.Equals(altId));

        public Task<Customer?> GetAsync(Guid altId) => _db.Customers.FirstOrDefaultAsync(c => c.AltId.Equals(altId));

        public IEnumerable<Customer> Get(Func<Customer, bool> filter) => this._db.Customers.Where(filter);

        public bool PermanentlyRemoveCustomer(Guid altId)
        {
            this._db.Customers
                .Where(c => c.AltId == altId)
                .ExecuteDelete();
            return true;
        }

        public int Update(Customer t, bool commit)
        {
            this._db.Customers.Update(t);
            return commit ? this._db.SaveChanges() : 0;
        }

        public IAsyncEnumerable<Customer> GetAsync(Func<Customer, bool> filter)
        {
            return this._db.Customers.Where(filter).AsQueryable().AsAsyncEnumerable();
        }

        public Task<int> GenerateAsync(Customer t)
        {
            if (t.Id != null)
            {
                return Task.FromResult(0);
            }
            t.AltId = Guid.NewGuid();
            this._db.Customers.Add(t);
            return this._db.SaveChangesAsync();
        }

        public Task<int> UpdateAsync(Customer t)
        {
            this._db.Customers.Update(t);
            return this._db.SaveChangesAsync();
        }

        public Task<int> LogicalDeleteAsync(Customer t)
        {
            if (t.AltId == null)
            {
                return Task.FromResult(0);
            }
            this._db.Track(t);
            t.Active = false;
            return this._db.SaveChangesAsync();
        }
    }
}
