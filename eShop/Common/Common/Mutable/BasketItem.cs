using Common.Interface;
using Common.Models.Immutable;

namespace Common.Models.Mutable;

public class BasketItem : IElement<BasketItem>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public bool Active { get; set; }
    public Product Product { get; set; }
    public Customer Customer { get; set; }
    public uint Quantity { get; set; }

    public U Visit<U>(IVisitor<BasketItem, U> visitor)
    {
        return visitor.Visit(this);
    }
}
