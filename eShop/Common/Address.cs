using Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common
{
    public class Address : IElement<Address>, ISecureElement
    {
        [JsonIgnore]
        public int? Id { get; set; }
        public Guid? AltId { get; set; }
        public string HouseNameNumber { get; set; }
        public IEnumerable<string>? AddressLines { get; set; }
        public string? CityTown { get; set; }
        public string? Region { get; set; }
        public string PostalCode { get; set; }
        public bool Active {  get; set; }
        public Address Visit(IVisitor<Address> visitor)
        {
            return visitor.Visit(this);
        }
    }
}
