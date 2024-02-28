using Common.Interface;
using Common;


namespace BusinessLayer.Interface.User
{
    public interface IAddressService : IGenerateUpdateDelete<Address>
    {
        Address? Get(Guid addressId);
    }
}
