using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IAddressDataAccess : IGenerateUpdateDelete<Address>
    {
        IAsyncEnumerable<Address> GetAllAsync(Guid custId, bool active);
        Task<Address?> GetAsync(Guid altId, bool active);
    }
}
