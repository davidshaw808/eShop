using Common.Interface;

namespace Common;

public class Product : IElement<Product>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public bool Active { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int? NumberInStock { get; set; }
    public int? RestrictedToAge { get; set; } 
    public Category? Category { get; set; }
    public IList<Review>? Reviews { get; set; }
    public U Visit<U>(IVisitor<Product,U> visitor)
    {
        return visitor.Visit(this);
    }
}
