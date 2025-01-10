using Common;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Implementation;

public class AddressDataAccess : IAddressDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext,Guid, Guid, bool,Task<Address?>> _addressGetterAsync;
    private readonly Func<eShopBaseContext, Guid, Guid, int?> _addressIdGetter;
    private readonly Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> _customerAddressGetterAsync;
    private readonly IDatabaseChangeValidation _dbChangeValidation;

    public AddressDataAccess (IDbContextUnitOfWorkDataAccess unitOfWork, IDatabaseChangeValidation dbChangeValidation) 
    {
        _db = unitOfWork.GetContext();
        _dbChangeValidation = dbChangeValidation;
        _addressGetterAsync = GetAsyncCompiledAddressSetter();
        _customerAddressGetterAsync = GetAsyncCompiledAddressGetter();
        _addressIdGetter = GetCompiledAddressId();
    }

    public async ValueTask<Address?> ReadAtomicAsync(Guid key)
    {
        return await _db.Addresses.FirstOrDefaultAsync(a => a.Key.Equals(key));
    }

    public void GenerateElementInUoW(Address address)
    {
        if (!_dbChangeValidation.Valid(address).Generate)
            return;
        address.GenerateForDatabase();
        _db.Addresses.Add(address);
    }
    
    public async ValueTask GenerateAtomicAsync(Address address)
    {
        if (!_dbChangeValidation.Valid(address).Generate)
            return;
        address.GenerateForDatabase();
        await _db.Addresses.SingleInsertAsync(address);//ValueTask continuation builder
    }

    public void UpdateElementInUoW(Address address)
    {
        if (!_dbChangeValidation.Valid(address).Generate)
            return;

        if (!address.Key.HasValue || !address.Customer.Key.HasValue)
            return;

        //possible to have multiple updates on the same object, if so and is currently tracked don't bother getting the Id from the db
        if (!_db.ExistsLocally(address))
        {
            var addressId = _addressIdGetter(_db, address.Customer.Key.Value, address.Key.Value);
            if (!addressId.HasValue)
                return;
            //set pk and start change tracking - as we know it's currently not being tracked
            address.Id = addressId.Value;
            _db.Attach(address);
        }
        _db.Addresses.Update(address);
    }

    public async ValueTask<int> UpdateAtomicAsync(Address address)
    {
        //note to self, ef core does not allow multiple concurrent
        //threads, also async updates are really only for leaf properties
        //it can't handle anything graph related easily.
        //Address should not update customer so there is no advantage using
        //ef core's bulk extension methods as all changes are leaf and atomic
        //methonds should not require the use of graph or change tracker
        
        if (!_dbChangeValidation.Valid(address).Update)
            return 0;

        if (!address.Key.HasValue || (address.Customer?.Key == null))
            return 0;

        return await _db.Addresses
                .Where(a => a.Key.Equals(address.Key.Value) && a.Customer.Key.Equals(address.Customer.Key))
                .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.HouseNameNumber, address.HouseNameNumber)
                .SetProperty(p => p.AddressLines, address.AddressLines)
                .SetProperty(p => p.Active, address.Active)
                );
    }

    public async ValueTask<Address?> GetAsync(Guid customerKey, Guid key, bool active) =>  await this._addressGetterAsync(_db, customerKey, key, active);

    public async ValueTask<IAsyncEnumerable<Address>> GetAllAsync(Guid custKey, bool active) => await this._customerAddressGetterAsync(this._db, custKey, active);

    public void LogicalDeleteElementInUow(Address address)
    {
        if (!_dbChangeValidation.Valid(address).Update)
            return;

        if (!address.Key.HasValue || !address.Customer.Key.HasValue)
            return;

        if (!_db.ExistsLocally(address))
        {
            var addressId = _addressIdGetter(_db, address.Customer.Key.Value, address.Key.Value);
            if (!addressId.HasValue)
                return;
            //set pk
            address.Id = addressId.Value;
        }
        _db.LogicalDelete(address);
    }

    public async ValueTask<int> LogicalDeleteAtomicAsync(Address address)
    {
        if (!_dbChangeValidation.Valid(address).Update)
            return 0;

        if (!address.Key.HasValue && !address.Customer.Key.HasValue)
            return 0;
        return await _db.Addresses
            .Where(a => a.Key.Equals(address.Key.Value) && a.Customer.Key.Equals(address.Customer.Key.Value))
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
    }

    private Func<eShopBaseContext, Guid, Guid, bool, Task<Address?>> GetAsyncCompiledAddressSetter() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid CustomerKey, Guid Key, bool active) => db.Addresses.FirstOrDefault(a => a.Key.Equals(Key) && a.Active.Equals(active) && a.Customer.Key.Equals(CustomerKey))
        );

    private Func<eShopBaseContext, Guid, Guid, int?> GetCompiledAddressId() => EF.CompileQuery(
        (eShopBaseContext db, Guid customerKey, Guid key) => db.Addresses
            .Where(a => a.Key.Equals(key) && a.Customer.Key.Equals(customerKey))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

    private Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> GetAsyncCompiledAddressGetter() => EF.CompileAsyncQuery(
            (eShopBaseContext db, Guid custKey, bool active) =>
            db.Addresses
            .Include(a => a.Customer)
            .Where(a => a.Customer.Key.Equals(custKey) && a.Active.Equals(active))
            .AsAsyncEnumerable());
}
