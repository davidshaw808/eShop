using Common;
using Common.Interface;

namespace DataLayer.Interface
{
    public interface IAddressDataAccess : IGenerateUpdateDelete<Address>
    {
        public Address? Get(Guid altId);
    }
}
