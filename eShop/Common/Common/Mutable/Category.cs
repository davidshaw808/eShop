using Common.Interface;
using Common.Models.Immutable;

namespace Common.Models.Mutable;

public class Category : IElement<Category>, IParentChild<Category>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public string? Name { get; set; }
    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; }
    public ICollection<Product> Products { get; set; }
    public bool Active { get; set; }

    public U Visit<U>(IVisitor<Category, U> visitor)
    {
        return visitor.Visit(this);
    }
}
