using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IAddressDataAccess : IUnitOfWorkCUD<Address>, IAtomicCRUD<Address>
{
    Task<IAsyncEnumerable<Address>> GetAllAsync(Guid customerKey, bool active);
    Task<Address?> GetAsync(Guid Key, bool active);
}
