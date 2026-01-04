using Common.Interface;

namespace Common.Models.Mutable;

public record Shipping : IElementImmutable<Shipping>
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string CountryCode { get; set; }
    public decimal Cost { get; set; }
    public bool Active { get; set; }
    public Guid Key { get; init; }
    DateTime Shipped { get; set; }

    public U Visit<U>(IVisitor<Shipping, U> visitor)
    {
        return visitor.Visit(this);
    }
}
