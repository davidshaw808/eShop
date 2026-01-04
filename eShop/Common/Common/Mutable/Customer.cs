using Common.Base;
using Common.Interface;

namespace Common.Models.Mutable;

public class Customer : Person, IElement<Customer>
{
    public DateTime? RemoveAllCustomerDataRequest { get; set; }

    public U Visit<U>(IVisitor<Customer, U> visitor)
    {
        return visitor.Visit(this);
    }
}
