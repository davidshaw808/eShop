using Common.Interface;
using System.Text.Json.Serialization;

namespace Common
{
    public class Address : AddressExtern, IInternalElement<AddressExtern>
    {
        [JsonIgnore]
        public int? Id { get; set; }
    }

    public class AddressExtern : ISecureElement, IElement<AddressExtern>
    {
        public Guid? AltId { get; set; }
        public string HouseNameNumber { get; set; }
        public IEnumerable<string>? AddressLines { get; set; }
        public string? CityTown { get; set; }
        public string? Region { get; set; }
        public string PostalCode { get; set; }
        public bool Active { get; set; }
        public AddressExtern Visit(IVisitor<AddressExtern> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
