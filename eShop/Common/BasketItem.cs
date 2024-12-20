using Common.Interface;

namespace Common;

public class BasketItem : IElement<BasketItem>
{
    public int? Id { get; set; }
    public Guid? Key { get; set; }
    public bool Active { get; set; }
    public Product Products { get; set; }
    public Customer Customer { get; set; }
    public int Quantity { get; set; }

    public U Visit<U>(IVisitor<BasketItem, U> visitor)
    {
        throw new NotImplementedException();
    }
}
