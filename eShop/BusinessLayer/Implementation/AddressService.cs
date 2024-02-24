
using BusinessLayer.Interface;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class AddressService: IAddressService
    {
        readonly IAddressDataAccess _addressDataAccess;
        public AddressService(IAddressDataAccess addressDataAccess) 
        {
            this._addressDataAccess = addressDataAccess;
        }

        public bool Delete(Address t)
        {
            if (t.Id == null)
            {
                return false;
            }
            return this._addressDataAccess.Delete(t);
        }

        public bool Generate(Address t)
        {
            if (string.IsNullOrWhiteSpace(t.HouseNameNumber) || string.IsNullOrWhiteSpace(t.PostalCode))
            {
                throw new InvalidDataException("house name/number and post code cannot be null");
            }
            return this._addressDataAccess.Generate(t);
        }

        public Address? Get(Guid addressId)
        {
            return this._addressDataAccess.Get(addressId);
        }

        public bool Update(Address t)
        {
            if (string.IsNullOrWhiteSpace(t.HouseNameNumber) || string.IsNullOrWhiteSpace(t.PostalCode))
            {
                throw new InvalidDataException("house name/number and post code cannot be null");
            }
            if(t.Id == null)
            {
                return false;
            }
            return this._addressDataAccess.Update(t);
        }
    }
}
