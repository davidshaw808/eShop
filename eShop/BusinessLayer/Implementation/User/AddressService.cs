using BusinessLayer.Interface.User;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation.User
{
    public class AddressService : AddressOrderService, IAddressService
    {
        readonly IAddressDataAccess _addressDataAccess;
        public AddressService(IAddressDataAccess addressDataAccess) : base(addressDataAccess)
        {
            _addressDataAccess = addressDataAccess;
        }

        public bool Delete(Address t)
        {
            if (t.Id == null)
            {
                return false;
            }
            return _addressDataAccess.Delete(t);
        }

        public Address? Get(Guid addressId)
        {
            return _addressDataAccess.Get(addressId);
        }

        public bool Update(Address t)
        {
            if (string.IsNullOrWhiteSpace(t.HouseNameNumber) || string.IsNullOrWhiteSpace(t.PostalCode))
            {
                throw new InvalidDataException("house name/number and post code cannot be null");
            }
            if (t.Id == null)
            {
                return false;
            }
            return _addressDataAccess.Update(t);
        }
    }
}
