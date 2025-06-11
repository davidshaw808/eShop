using Common.Interface;

namespace Common.Models.Mutable;

public class Product : IElement<Product>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
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
