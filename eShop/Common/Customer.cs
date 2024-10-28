using Common.Base;
using Common.Interface;

namespace Common;

public class Customer : Person, IElement<Customer>
{
    public int? Id { get; set; }

    public DateTime? RemoveAllCustomerDataRequest { get; set; }

    public U Visit<U>(IVisitor<Customer,U> visitor)
    {
        return visitor.Visit(this);
    }
}
