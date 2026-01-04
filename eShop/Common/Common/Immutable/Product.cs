using Common.Interface;
using Common.Models.Mutable;

namespace Common.Models.Immutable;

public sealed record Product : IElementImmutable<Product>
{
    public int Id { get; init; }
    public Guid Key { get; init; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal NetPrice { get; set; }
    public bool Vatable { get; set; }
    public uint? NumberInStock { get; set; }
    public uint? RestrictedToAge { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<Review> Reviews { get; set; }
    public U Visit<U>(IVisitor<Product, U> visitor)
    {
        return visitor.Visit(this);
    }
}
