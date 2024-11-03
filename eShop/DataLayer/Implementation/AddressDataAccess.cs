using Common;
using DataLayer.Databases.Base;
using DataLayer.Implementation.Extensions;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace DataLayer.Implementation;

public class AddressDataAccess : IAddressDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext,Guid,bool,Task<Address?>> _addressGetterAsync;
    private readonly Func<eShopBaseContext, Guid, int?> _addressIdGetter;
    private readonly Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> _customerAddressGetterAsync;

    public AddressDataAccess(IDbContextUnitOfWorkDataAccess unitOfWork)
    {
        _db = unitOfWork.GetContext();
        _addressGetterAsync = GetAsyncCompiledAddressSetter();
        _customerAddressGetterAsync = GetAsyncCompiledAddressGetter();
        _addressIdGetter = GetCompiledAddressId();
    }

    public Task<Address?> ReadAtomicAsync(Guid key)
    {
        return _db.Addresses.FirstOrDefaultAsync(a => a.Key.Equals(key));
    }

    public void GenerateElementInUoW(Address address)
    {
        Generate(address);
        _db.Addresses.Add(address);
    }

    public Task GenerateAtomicAsync(Address address)
    {
        Generate(address);
        return _db.Addresses.SingleInsertAsync(address);
    }

    public void UpdateElementInUoW(Address address)
    {
        if (!address.Key.HasValue)
            return;
        //possible to have multiple updates on the same object, if so and is currently tracked don't bother getting the Id from the db
        if (!_db.ExistsLocally(address))
        {
            var addressId = _addressIdGetter(_db, address.Key.Value);
            if (!addressId.HasValue)
                return;
            //set pk and start change tracking - as we know it's currently not being tracked
            address.Id = addressId.Value;
            _db.Attach(address);
        }
        _db.Addresses.Update(address);
    }

    public Task<int> UpdateAtomicAsync(Address address)
    {
        if (!address.Key.HasValue)
            return StaticExtensions.Uncommitted;
        return _db.Addresses
            .Where(a => a.Key.Equals(address.Key.Value))
            //.Include(a => a.Customer)--not needed just an address update
            .ExecuteUpdateAsync( setter => setter
            .SetProperty(p => p.HouseNameNumber, address.HouseNameNumber)
            .SetProperty(p => p.AddressLines, address.AddressLines)
            .SetProperty(p => p.Active, address.Active)
            .SetProperty(p => p.CityTown, address.CityTown) 
            );
    }

    public Task<Address?> GetAsync(Guid Key, bool active) =>  this._addressGetterAsync(_db, Key, active);

    public Task<IAsyncEnumerable<Address>> GetAllAsync(Guid custId, bool active) => this._customerAddressGetterAsync(this._db, custId, active);

    public void LogicalDeleteElementInUow(Address address)
    {
        if(!address.Key.HasValue)
            return;
        if (!_db.ExistsLocally(address))
        {
            var addressId = _addressIdGetter(_db, address.Key.Value);
            if (!addressId.HasValue)
                return;
            //set pk
            address.Id = addressId.Value;
        }
        _db.LogicalDelete(address);
    }

    public Task<int> LogicalDeleteAtomicAsync(Address address)
    {
        if (!address.Key.HasValue)
            return StaticExtensions.Uncommitted;
        return _db.Addresses
            .Where(a => a.Key.Equals(address.Key.Value))
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
    }

    private Func<eShopBaseContext, Guid, bool, Task<Address?>> GetAsyncCompiledAddressSetter() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid Key, bool active) => db.Addresses.FirstOrDefault(a => a.Key.Equals(Key) && a.Active.Equals(active))
        );

    private Func<eShopBaseContext, Guid, int?> GetCompiledAddressId() => EF.CompileQuery(
        (eShopBaseContext db, Guid Key) => db.Addresses
            .Where(a => a.Key.Equals(Key))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

    private Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> GetAsyncCompiledAddressGetter() => EF.CompileAsyncQuery(
            (eShopBaseContext db, Guid custKey, bool active) =>
            db.Addresses
            .Include(a => a.Customer)
            .Where(a => a.Customer.Key.Equals(custKey) && a.Active.Equals(active))
            .AsAsyncEnumerable());

    private void Generate(Address address)
    {
        address.Id = null;
        address.Key = Guid.NewGuid();
    }
}
