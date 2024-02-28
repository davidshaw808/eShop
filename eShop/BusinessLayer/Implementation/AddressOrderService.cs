using BusinessLayer.Interface.User;
using Common;
using DataLayer.Interface;

namespace BusinessLayer.Implementation
{
    public class AddressOrderService : IAddressOrderService
    {
        readonly IAddressDataAccess _addressDataAccess;
        public AddressOrderService(IAddressDataAccess addressDataAccess)
        {
            _addressDataAccess = addressDataAccess;
        }

        public bool Generate(Address t)
        {
            if (string.IsNullOrWhiteSpace(t.HouseNameNumber) || string.IsNullOrWhiteSpace(t.PostalCode))
            {
                throw new InvalidDataException("house name/number and post code cannot be null");
            }
            return this._addressDataAccess.Generate(t);
        }
    }
}
