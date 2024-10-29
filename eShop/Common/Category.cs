using Common.Interface;

namespace Common;

public class Category : IElement<Category>, IParentChild<Category>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public int? ParentId { get; set; }
    public string? Name { get; set; }
    public Category? Parent { get; set; }
    public IList<Category> Children { get; set; }
    public IList<Product> Products { get; set; }
    public bool Active { get; set; }

    public U Visit<U>(IVisitor<Category, U> visitor)
    {
        return visitor.Visit(this);
    }
}
