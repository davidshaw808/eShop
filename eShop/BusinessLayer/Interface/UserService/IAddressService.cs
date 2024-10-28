using Common.Interface;
using Common;


namespace BusinessLayer.Interface.User;

public interface IAddressService : IGenerateUpdateDelete<AddressInternal>
{
    AddressInternal? Get(Guid addressId);
}
