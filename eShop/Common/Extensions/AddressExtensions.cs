using Common.Models.Mutable;

namespace Common.Extensions;

public static class AddressExtensions
{
    private static string Concat(params IEnumerable<object> args) => string.Join(", ", args.Where(a => a is not null).Select(a => a.ToString()));

    public static string ToString(this Address address)
    {
        return Concat(address.HouseNameNumber, address.AddressLines, address.CityTown, address.Region, address.PostalCode, address.Country);
    }
}
