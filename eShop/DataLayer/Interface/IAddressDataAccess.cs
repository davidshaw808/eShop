using Common;
using DataLayer.Interface.General;

namespace DataLayer.Interface;

public interface IAddressDataAccess : IUnitOfWorkCUD<Address>, IAtomicCRUD<Address>
{
    ValueTask<IAsyncEnumerable<Address>> GetAllAsync(Guid customerKey, bool active);
    ValueTask<Address?> GetAsync(Guid customerKey, Guid Key, bool active);
}
