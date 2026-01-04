using Common.Models.Mutable;
using DataLayer.ClassHelpers.Extensions;
using DataLayer.Databases.Base;
using DataLayer.Interface;
using DataLayer.Interface.General;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DataLayer.Implementation;

public class AddressDataAccess : IAddressDataAccess
{
    private readonly eShopBaseContext _db;
    private readonly Func<eShopBaseContext, Guid, Task<Address?>> _atomicAdressGetterAsync;
    private readonly Func<eShopBaseContext, Guid, Task<Address?>> _uowAdressGetterAsync;
    private readonly Func<eShopBaseContext, Guid, Guid, Task<int?>> _addressIdGetter;
    private readonly Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> _getCustomerAddressAsync;


    public AddressDataAccess (IDbContextUnitOfWorkDataAccess unitOfWork) 
    {
        _db = unitOfWork.GetContext();
        _getCustomerAddressAsync = GetCompiledCustomerAddressTrackingAsync();
        _addressIdGetter = GetCompiledAddressIdAsync();
        _atomicAdressGetterAsync = GetCompiledAddressTrackingAsync();
        _uowAdressGetterAsync = GetCompiledCustomerAddressNoTrackingAsync();
    }

    public async Task<Address?> GetAtomicAsync(Guid key) => await _atomicAdressGetterAsync(_db, key);
    
    public async Task<Address?> GetElementInUoW(Guid key) => await _uowAdressGetterAsync(_db, key);

    public void GenerateElementInUoW(Address address)
    {
        address.GenerateForDatabase();
        _db.Addresses.Add(address);
    }
    
    public async Task GenerateAtomicAsync(Address address)
    {
        address.GenerateForDatabase();
        await _db.Addresses.SingleInsertAsync(address);//Task continuation builder
    }

    public async Task UpdateElementInUoW(Address address)
    {
        if (!address.Key.HasValue || !address.Customer.Key.HasValue)
            return;

        //possible to have multiple updates on the same object, if so and is currently tracked don't bother getting the Id from the db
        if (!_db.ExistsLocally(address))
        {
            var addressId = await _addressIdGetter(_db, address.Customer.Key.Value, address.Key.Value);
            if (!(addressId > 0))
                return;
            //set pk and start change tracking - as we know it's currently not being tracked
            address.Id = addressId.Value;
        }
        _db.Addresses.Update(address);
    }

    public async Task<bool> UpdateAtomicAsync(Address address)
    {
        //note to self, ef core does not allow multiple concurrent
        //threads, also async updates are really only for leaf properties
        //it can't handle anything graph related easily.
        //Address should not update customer so there is no advantage using
        //ef core's bulk extension methods as all changes are leaf and atomic
        //methonds should not require the use of graph or change tracker

        if (!address.Key.HasValue || (address.Customer?.Key == null))
            return false;

        return (await _db.Addresses
                .Where(a => a.Key.Equals(address.Key.Value) && a.Customer.Key.Equals(address.Customer.Key))
                .ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.HouseNameNumber, address.HouseNameNumber)
                .SetProperty(p => p.AddressLines, address.AddressLines)
                .SetProperty(p => p.Active, address.Active)
                .SetProperty(p => p.CityTown, address.CityTown)
                .SetProperty(p => p.Country, address.Country)
                )) > 0;
    }

    public async Task<IAsyncEnumerable<Address>> GetAllAsync(Guid custKey, bool active) => await this._getCustomerAddressAsync(_db, custKey, active);

    public async Task LogicalDeleteElementInUow(Address address)
    {
        if (!address.Key.HasValue || !address.Customer.Key.HasValue)
            return;

        if (!_db.ExistsLocally(address))
        {
            var addressId = await _addressIdGetter(_db, address.Customer.Key.Value, address.Key.Value);
            if (!addressId.HasValue)
                return;
            //set pk
            address.Id = addressId.Value;
        }
        _db.LogicalDelete(address);
    }

    public async Task<bool> LogicalDeleteAtomicAsync(Address address)
    {
        if (!address.Key.HasValue && !address.Customer.Key.HasValue)
            return false;
        var affected = await _db.Addresses
            .Where(a => a.Key.Equals(address.Key.Value) && a.Customer.Key.Equals(address.Customer.Key.Value))
            .ExecuteUpdateAsync(a => a.SetProperty(p => p.Active, false));
        return affected > 0;
    }


    private Func<eShopBaseContext, Guid, Guid, Task<int?>> GetCompiledAddressIdAsync() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid customerKey, Guid key) => db.Addresses
            .Where(a => a.Key.Equals(key) && a.Customer.Key.Equals(customerKey))
            .Select(a => a.Id)
            .FirstOrDefault()
        );

    private Func<eShopBaseContext, Guid, bool, Task<IAsyncEnumerable<Address>>> GetCompiledCustomerAddressTrackingAsync() => EF.CompileAsyncQuery(
            (eShopBaseContext db, Guid custKey, bool active) =>
            db.Addresses
            .Include(a => a.Customer)
            .Where(a => a.Customer.Key.Equals(custKey) && a.Active.Equals(active))
            .AsAsyncEnumerable());

    private Func<eShopBaseContext, Guid, Task<Address?>> GetCompiledCustomerAddressNoTrackingAsync() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid key) => db.Addresses
            .Include(a => a.Customer)
            .FirstOrDefault(a => a.Key.Equals(key))
        );

    private Func<eShopBaseContext, Guid, Task<Address?>> GetCompiledAddressTrackingAsync() => EF.CompileAsyncQuery(
        (eShopBaseContext db, Guid key) => db.Addresses
            .AsNoTrackingWithIdentityResolution()
            .Include(a => a.Customer)
            .FirstOrDefault(a => a.Key.Equals(key))
        );
}
