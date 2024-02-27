using Common.Interface;
using Common;


namespace BusinessLayer.Interface
{
    public interface IAddressService: IGenerateUpdateDelete<Address>
    {
        Address? Get(Guid addressId);
    }
}
