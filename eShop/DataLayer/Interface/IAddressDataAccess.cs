using Common.Models.Mutable;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IAddressDataAccess : IUnitOfWorkCRUD<Address>, IAtomicCRUD<Address>
{
    Task<IAsyncEnumerable<Address>> GetAllAsync(Guid customerKey, bool active);
    Task<Address?> GetAsync(Guid customerKey, Guid Key, bool active);
}
