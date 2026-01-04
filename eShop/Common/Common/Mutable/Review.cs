using Common.Base;
using Common.Interface;
using Common.Models.Immutable;

namespace Common.Models.Mutable;

public class Review : IElement<Review>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public int? ProductId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Product? Product { get; set; }
    public Person Owner { get; set; }
    public bool Active { get; set; }

    public U Visit<U>(IVisitor<Review, U> visitor)
    {
        return visitor.Visit(this);
    }
}