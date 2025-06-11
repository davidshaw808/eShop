using Common.Models.Mutable;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Z.BulkOperations;

namespace DataLayer.Implementation;

public class CustomerDataAccess : ICustomerDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext, Guid, Task<int?>> _customerIdGetter;
    private readonly Func<eShopBaseContext, Guid, Task<Customer?>> _customerGetterNoTrackAsync;
    private readonly Func<eShopBaseContext, string, Task<Customer?>> _customerByEmailNoTrackAsync;
    private readonly Func<eShopBaseContext, Guid, bool, Task<Customer?>> _customersAllGetterTrackingAsync;
    private readonly Func<eShopBaseContext, Guid, Task<Customer?>> _customerGetterTrackingAsync;

    public CustomerDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
    {
        _db = unitOfWork.GetContext();
        _customerIdGetter = GetCompiledCustomerId();
        _customerGetterNoTrackAsync = GetCompiledCustomerByKeyNoTrack();
        _customerByEmailNoTrackAsync = GetCompiledCustomerByEmailNoTrack();
        _customersAllGetterTrackingAsync = GetAsyncCompiledCustomerSetter();
        _customerGetterTrackingAsync = GetCompiledCustomerByKeyTracking();
    }

    public IEnumerable<Customer> Get(Func<Customer, bool> filter) => this._db.Customers.Where(filter);

    public IAsyncEnumerable<Customer> GetAsync(Func<Customer, bool> filter)
    {
        return this._db.Customers.Where(filter).AsQueryable().AsAsyncEnumerable();
    }

    public void GenerateElementInUoW(Customer customer)
    {
        customer.GenerateForDatabase();
        _db.Customers.Add(customer);
    }

    public async Task GenerateAtomicAsync(Customer customer)
    {
        customer.GenerateForDatabase();
        await _db.Customers.SingleInsertAsync(customer);
    }

    public async Task LogicalDeleteElementInUow(Customer customer)
    {
        if (!customer.Key.HasValue)
            return;

        if (!_db.ExistsLocally(customer))
        {
            var customerId = await _customerIdGetter(_db, customer.Key.Value);
            if (!(customerId > 0))
                return;
            //set pk
            customer.Id = customerId.Value;
        }
        _db.LogicalDelete(customer);
    }

    public async Task<Customer?> GetAtomicAsync(Guid key) => await _customerGetterNoTrackAsync(_db, key);
    public async Task<Customer?> GetAtomicAsync(string email) => await _customerByEmailNoTrackAsync(_db, email);
    public async Task<Customer?> GetElementInUoW(Guid key) => await _customerGetterTrackingAsync(_db, key);

    public async Task<bool> PermanentlyRemoveCustomerDetailsAsync(Customer customer)
    {
        var bulkOptions = (BulkOperation<Customer> options) =>
        {
            options.ColumnPrimaryKeyExpression = c => c.Key;
            options.IgnoreOnUpdateExpression = c => c.Id;
            options.AllowUpdatePrimaryKeys = false;
            options.IncludeGraph = false;
        };

        var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            //reminder EF Core is not thread safe - so run these as concurrent continuations
            await _db.Addresses
                .Where(a => a.Customer.Key.Equals(customer.Key))
                .ExecuteDeleteAsync();

            await _db.Customers.SingleDeleteAsync(customer, bulkOptions);
            
            await transaction.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> UpdateAtomicAsync(Customer customer)
    {
        //note to self, ef core does not allow multiple concurrent
        //threads, also async updates are really only for leaf properties
        //it can't handle anything graph related easily.
        //Address should not update customer so there is no advantage using
        //ef core's bulk extension methods as all changes are leaf and atomic
        //methonds should not require the use of graph or change tracker

        if (!customer.Key.HasValue)
            return false;

        Task update = _db.Customers.SingleUpdateAsync(customer, options =>
        {
            options.ColumnPrimaryKeyExpression = c => c.Key;
            options.IgnoreOnUpdateExpression = c => c.Id;
            options.AllowUpdatePrimaryKeys = false;
            options.IncludeGraph = true;
            options.IncludeGraphOperationBuilder = operation =>
            {
                if (operation is BulkOperation<Address>)
                {
                    var bulk = (BulkOperation<Address>)operation;
                    bulk.ColumnPrimaryKeyExpression = x => x.Key;
                }
            };
        });
        await update;
        return update.IsCompletedSuccessfully;
    }

    public async Task<bool> LogicalDeleteAtomicAsync(Customer customer)
    {
        var affected = await _db.Customers
            .Where(c => c.Key.Equals(customer.Key))
            .ExecuteUpdateAsync(c => c.SetProperty(p => p.Active, false));
        return affected > 0;
    }

    public async Task UpdateElementInUoW(Customer customer)
    {
        if (!customer.Key.HasValue)
            return;

        if (!_db.ExistsLocally(customer))
        {
            var customerId = await _customerIdGetter(_db, customer.Key.Value);
            if (!(customerId > 0))
                return;
            //set pk
            customer.Id = customerId.Value;
        }
        _db.Customers.Update(customer);
    }

    private Func<eShopBaseContext, Guid, Task<Customer?>> GetCompiledCustomerByKeyTracking() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key) => db.Customers
        .Include(c => c.Address)
        .Include(c => c.BasketItems)
        .Include(c => c.OrderHistory)
        .Where(a => a.Key.Equals(Key))
        .FirstOrDefault()
    );

    private Func<eShopBaseContext, Guid, Task<Customer?>> GetCompiledCustomerByKeyNoTrack() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key) => db.Customers
        .AsNoTracking()
        .Include(c => c.Address)
        .Include(c => c.BasketItems)
        .Include(c => c.OrderHistory)
        .Where(a => a.Key.Equals(Key))
        .FirstOrDefault()
    );

    private Func<eShopBaseContext, string, Task<Customer?>> GetCompiledCustomerByEmailNoTrack() => EF.CompileAsyncQuery(
        (eShopBaseContext db, string email) => db.Customers
        .AsNoTracking()
        .Include(c => c.Address)
        .Include(c => c.BasketItems)
        .Include(c => c.OrderHistory)
        .Where(a => a.Email != null &&  a.Email.Equals(email))
        .FirstOrDefault()
    );

    private Func<eShopBaseContext, Guid, Task<int?>> GetCompiledCustomerId() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key) => db.Customers
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

    private Func<eShopBaseContext, Guid, bool, Task<Customer?>> GetAsyncCompiledCustomerSetter() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key, bool active) => db.Customers
        .Include(c => c.Address)
        .Include(c => c.BasketItems)
        .Include(c => c.OrderHistory)
        .FirstOrDefault(c => c.Key.Equals(Key))
        );

   
}
