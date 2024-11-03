using Common.Interface;
using System.Text.Json.Serialization;

namespace Common;

public class Address : IElement<Address>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public Customer Customer { get; set; }
    public string HouseNameNumber { get; set; }
    public IEnumerable<string>? AddressLines { get; set; }
    public string? CityTown { get; set; }
    public string? Region { get; set; }
    public string PostalCode { get; set; }
    public bool Active { get; set; }


    public U Visit<U>(IVisitor<Address, U> visitor)
    {
        return visitor.Visit(this);
    }
}
