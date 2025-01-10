using Common;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace DataLayer.Implementation;

public class CustomerDataAccess : ICustomerDataAccess
{
    private readonly eShopBaseContext _db;

    public CustomerDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
    {
        this._db = unitOfWork.GetContext();
    }

    public int LogicalDelete(Customer t, bool commit)
    {
        this._db.LogicalDelete(t);
        return commit ? this._db.SaveChanges() : 0;
    }

    public int Generate(Customer t, bool commit)
    {
        if (t.Id != null)
        {
            return 0;
        }
        t.Key = Guid.NewGuid();
        this._db.Customers.AddAsy(t);
        return commit ? this._db.SaveChanges() : 0;
    }

    public Task<Customer?> GetAsync(Guid Key) => _db.Customers.FirstOrDefaultAsync(c => c.Key.Equals(Key));

    public IEnumerable<Customer> Get(Func<Customer, bool> filter) => this._db.Customers.Where(filter);

    public Task<int> PermanentlyRemoveCustomer(Guid Key)
    {
        return this._db.Customers
            .Where(c => c.Key == Key)
            .DeleteFromQueryAsync();
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
        t.Key = Guid.NewGuid();
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
        if (t.Key == null)
        {
            return Task.FromResult(0);
        }
        this._db.Track(t);
        t.Active = false;
        return this._db.SaveChangesAsync();
    }

    private Func<eShopBaseContext, Guid, int?> GetCompiledCustomerId() => EF.CompileQuery(
        (eShopBaseContext db, Guid Key) => db.Customers
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

    public void GenerateElementInUoW(Customer customer)
    {
        customer.GenerateForDatabase();
    }
    public Task GenerateAtomicAsync(Customer t)
    {
        throw new NotImplementedException();
    }

    public void UpdateElementInUoW(Customer t)
    {
        throw new NotImplementedException();
    }

    public void LogicalDeleteElementInUow(Customer t)
    {
        throw new NotImplementedException();
    }

    
    public Task<Customer?> ReadAtomicAsync(Guid key)
    {
        throw new NotImplementedException();
    }


    public Task<int> LogicalDeleteAtomicAsync(Customer t)
    {
        throw new NotImplementedException();
    }
}
